// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public class DocumentManager
    {
        #region DI
        public string[] StartTrimValues = new string[] { "Database\\" };
        public DocumentSettings DocumentSettings { get; private set; }
        public string RootPath { get; private set; }


        #endregion

        #region ctor's
        public DocumentManager(DocumentSettings documentSettings, string rootPath)
        {
            DocumentSettings = documentSettings;
            RootPath = rootPath;
        }

        #endregion

        #region  search


        public DocumentSearchResult Search(DoumentFilter filter)
        {
            DocumentSearchResult documentSearchResult = new DocumentSearchResult();

            var folder = GetACObjectFolder(filter.ACTypeACUrl, filter.ACObjectACUrl);


            documentSearchResult.Filter = filter;
            return documentSearchResult;
        }

        public byte[] Get(string aCTypeACUrl, string aCObjectACUrl, string fileName)
        {
            var folder = GetACObjectFolder(aCTypeACUrl, aCObjectACUrl);
            string path = Path.Combine(folder, fileName);
            return File.ReadAllBytes(path);
        }
        #endregion

        #region Manipulation
        public void Add(string aCTypeACUrl, string aCObjectACUrl, string fileName, byte[] content)
        {
            var folder = GetACObjectFolder(aCTypeACUrl, aCObjectACUrl);
            string path = Path.Combine(folder, fileName);
            if (File.Exists(path))
                File.Delete(path);
            using (MemoryStream ms = new MemoryStream(content))
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fs))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(ms))
                        {
                            byte value = 0;
                            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                            {
                                value = binaryReader.ReadByte();
                                binaryWriter.Write(value);
                            }
                        }
                    }
                }
            }
        }

        public void Delete(string aCTypeACUrl, string aCObjectACUrl, string fileName)
        {
            var folder = GetACObjectFolder(aCTypeACUrl, aCObjectACUrl);
            string path = Path.Combine(folder, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        public bool Exist(string aCTypeACUrl, string aCObjectACUrl, string fileName)
        {
            var folder = GetACObjectFolder(aCTypeACUrl, aCObjectACUrl);
            string path = Path.Combine(folder, fileName);
            return File.Exists(path);
        }

        public void DeleteAll(string aCTypeACUrl, string aCObjectACUrl)
        {
            var folder = GetACObjectFolder(aCTypeACUrl, aCObjectACUrl);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }

        public string GetRootTypeFolder(string aCTypeACUrl)
        {
            string[] folders = aCTypeACUrl.Split('\\');
            string tmpFolder = RootPath;
            foreach (var folder in folders)
            {
                tmpFolder = Path.Combine(tmpFolder, folder);
                if (!Directory.Exists(tmpFolder))
                    Directory.CreateDirectory(tmpFolder);
            }
            return tmpFolder;
        }

        public string GetACObjectFolder(string aCTypeACUrl, string aCObjectACUrl)
        {
            string folder = GetRootTypeFolder(aCTypeACUrl);
            var tmp = Path.Combine(folder, aCObjectACUrl);
            if (!Directory.Exists(tmp))
                Directory.CreateDirectory(tmp);
            return tmp;
        }

        public string StartTrimValuesGet(string input)
        {
            foreach (var startTrimValue in StartTrimValues)
                input = input.TrimStart(startTrimValue.ToCharArray());
            return input;
        }
        #endregion
    }
}
