using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Webkit;
using System;
using System.IO;
using System.Threading.Tasks;
using AndroidUri = Android.Net.Uri;

namespace gip.iplus.client.avui.Android;

/// <summary>
/// Hands files that were delivered by the server (e.g. a PDF of BSOMedia) to the viewer apps of the device.
/// Since Android 7 a file:// URI of the app cache must not leave the app, so the file is shared
/// with a content:// URI and a temporary read grant.
/// Registered as gip.core.layoutengine.avui.Helperclasses.VBFileOpener.PlatformHandler in <see cref="Application"/>.
/// </summary>
[ContentProvider(new[] { Authority }, Exported = false, GrantUriPermissions = true)]
public class OpenFileProvider : ContentProvider
{
    // Must match the ApplicationId of the project
    public const string Authority = "com.CompanyName.gip.mes.client.avui.openfile";

    private static string RootFolder
    {
        get { return Path.Combine(global::Android.App.Application.Context.CacheDir!.AbsolutePath, "openfile"); }
    }

    public static Task<bool> OpenAsync(string fileName, string mimeType, byte[] content)
    {
        try
        {
            Context context = global::Android.App.Application.Context;
            DeleteOldFiles();

            string id = Guid.NewGuid().ToString("N");
            string name = Path.GetFileName(fileName);
            string folder = Path.Combine(RootFolder, id);
            Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, name), content);

            if (string.IsNullOrEmpty(mimeType))
                mimeType = MimeTypeFromName(name) ?? "*/*";

            AndroidUri uri = AndroidUri.Parse("content://" + Authority + "/" + id + "/" + AndroidUri.Encode(name))!;
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, mimeType);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
            context.StartActivity(intent);
            return Task.FromResult(true);
        }
        catch (ActivityNotFoundException)
        {
            // No app installed that can show this file type
            return Task.FromResult(false);
        }
    }

    private static void DeleteOldFiles()
    {
        try
        {
            if (!Directory.Exists(RootFolder))
                return;
            foreach (string folder in Directory.GetDirectories(RootFolder))
            {
                if (Directory.GetCreationTimeUtc(folder) < DateTime.UtcNow.AddDays(-1))
                    Directory.Delete(folder, true);
            }
        }
        catch (Exception)
        {
        }
    }

    private static string? MimeTypeFromName(string name)
    {
        string extension = Path.GetExtension(name).TrimStart('.');
        return MimeTypeMap.Singleton?.GetMimeTypeFromExtension(extension.ToLowerInvariant());
    }

    private static FileInfo? ResolveFile(AndroidUri uri)
    {
        var segments = uri.PathSegments;
        if (segments == null || segments.Count != 2)
            return null;
        // Only a file name inside the folder of the file is allowed (no path traversal)
        string id = Path.GetFileName(segments[0]!);
        string name = Path.GetFileName(segments[1]!);
        FileInfo file = new FileInfo(Path.Combine(RootFolder, id, name));
        return file.Exists ? file : null;
    }

    public override bool OnCreate()
    {
        return true;
    }

    public override global::Android.OS.ParcelFileDescriptor? OpenFile(AndroidUri uri, string mode)
    {
        FileInfo? file = ResolveFile(uri);
        if (file == null)
            throw new Java.IO.FileNotFoundException(uri.ToString());
        return global::Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(file.FullName), global::Android.OS.ParcelFileMode.ReadOnly);
    }

    public override ICursor? Query(AndroidUri uri, string[]? projection, string? selection, string[]? selectionArgs, string? sortOrder)
    {
        FileInfo? file = ResolveFile(uri);
        if (file == null)
            return null;
        // Viewer apps ask for the display name and the size
        string[] columns = projection ?? new[] { IOpenableColumns.DisplayName, IOpenableColumns.Size };
        MatrixCursor cursor = new MatrixCursor(columns);
        MatrixCursor.RowBuilder? row = cursor.NewRow();
        foreach (string column in columns)
        {
            if (column == IOpenableColumns.DisplayName)
                row?.Add(file.Name);
            else if (column == IOpenableColumns.Size)
                row?.Add(file.Length);
            else
                row?.Add(null);
        }
        return cursor;
    }

    public override string? GetType(AndroidUri uri)
    {
        FileInfo? file = ResolveFile(uri);
        return file == null ? null : MimeTypeFromName(file.Name);
    }

    public override AndroidUri? Insert(AndroidUri uri, ContentValues? values)
    {
        throw new Java.Lang.UnsupportedOperationException();
    }

    public override int Delete(AndroidUri uri, string? selection, string[]? selectionArgs)
    {
        throw new Java.Lang.UnsupportedOperationException();
    }

    public override int Update(AndroidUri uri, ContentValues? values, string? selection, string[]? selectionArgs)
    {
        throw new Java.Lang.UnsupportedOperationException();
    }
}
