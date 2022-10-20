using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    public class DocumentSettings
    {
        #region ctor's
        public DocumentSettings()
        {
            DocumentTypeExtensions = new Dictionary<DocumentTypesEnum, List<string>>();

            Push(ExtensionsImage);
            Push(ExtensionsAudio);
            Push(ExtensionsVideo);
            Push(ExtensionsPDF);
            Push(ExtensionsDocuments);
            Push(ExtensionsSpreedSheet);
            Push(ExtensionsPresentation);
            Push(ExtensionsCompressed);
        }
        #endregion

        #region Definition

        public Dictionary<DocumentTypesEnum, List<string>> DocumentTypeExtensions { get; private set; }

        #endregion

        #region private loaders

        #endregion

        #region definitions
        public KeyValuePair<DocumentTypesEnum, string> ExtensionsImage = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.Image, @".ai .bmp .gif .ico .jpeg .jpg .png .ps .psd .svg .tif");
        public KeyValuePair<DocumentTypesEnum, string> ExtensionsAudio = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.Audio, @".aif .cda .mid .mp3 .mpa .ogg .wav .wma .wpi");
        public KeyValuePair<DocumentTypesEnum, string> ExtensionsVideo = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.Video, @".3g2 .3gp .avi .flv .h264 .m4v .mkv .mov .mp4 .mpg .mpeg .rm .swf .vob .wmv");

        public KeyValuePair<DocumentTypesEnum, string> ExtensionsPDF = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.PDF, @".pdf");
        public KeyValuePair<DocumentTypesEnum, string> ExtensionsDocuments = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.Documents, @".doc .docx .rtf .odt");
        public KeyValuePair<DocumentTypesEnum, string> ExtensionsSpreedSheet = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.SpreedSheet, @".xls .xlsx .ods");
        public KeyValuePair<DocumentTypesEnum, string> ExtensionsPresentation = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.Presentation, @".pps .ppt .pptx .odp");

        public KeyValuePair<DocumentTypesEnum, string> ExtensionsCompressed = new KeyValuePair<DocumentTypesEnum, string>(DocumentTypesEnum.Compressed, @".7z .arj .deb .pkg .rar .rpm .tar.gz .z .zip");

        #endregion

        #region private methods
        private KeyValuePair<DocumentTypesEnum, List<string>> Parse(KeyValuePair<DocumentTypesEnum, string> input)
        {
            List<string> extensions = input.Value.Split(' ').Select(c => c.Trim()).ToList();
            return new KeyValuePair<DocumentTypesEnum, List<string>>(input.Key, extensions);
        }

        private void Push(KeyValuePair<DocumentTypesEnum, string> input)
        {
            var extensions = Parse(input);
            DocumentTypeExtensions.Add(extensions.Key, extensions.Value);
        }
        #endregion
    }
}
