using gip.bso.iplus;
using gip.core.datamodel;
using System.Linq;

namespace gip.mes.cmdlet.DesignSync
{

    public class ACClassExporter : ACClassSyncBase
    {

        #region DI inputs


        public ExportCommand ExportCommand { get; set; }

        #endregion

        #region ctor's
        public ACClassExporter(string rootFolder, ACProjectManager projectManager, Database database, ExportCommand exportCommand)
        {
            RootFolder = rootFolder;
            ProjectManager = projectManager;
            Database = database;
            ExportCommand = exportCommand;
        }

        public void Export(ACProject project, string[] acIdentifers)
        {
            LoadProjectTree(project, acIdentifers);
            ACEntitySerializer aCEntitySerializer = new ACEntitySerializer();
            ACQueryDefinition qryACProject = gip.core.datamodel.Database.Root.Queries.CreateQuery(Database as IACComponent, Const.QueryPrefix + ACProject.ClassName, null);
            ACQueryDefinition qryACClass = qryACProject.ACUrlCommand(Const.QueryPrefix + ACClass.ClassName) as ACQueryDefinition;

            SendImportMessage(string.Format("Start export {0} to folder{1}...", acIdentifers, RootFolder));
            ExportCommand.DoExport(null, null, aCEntitySerializer, qryACProject, qryACClass, project, ProjectManager.CurrentProjectItemRoot, RootFolder, 0, 0);
        }


        private void LoadProjectTree(ACProject project, string[] acIdentifers)
        {
            ACProjectManager.PresentationMode projectTreePresentationMode
                                = new ACProjectManager.PresentationMode()
                                {
                                    ShowCaptionInTree = false,
                                    DisplayGroupedTree = true
                        //DisplayTreeAsMenu = this.CurrentMenuRootItem
                    };

            ACClassInfoWithItems.VisibilityFilters projectTreeVisibilityFilter
                  = new ACClassInfoWithItems.VisibilityFilters()
                  {
                      SearchText = null,
                      IncludeLibraryClasses = false
                  };
            core.datamodel.ACClassInfoWithItems.CheckHandler projectTreeCheckHandler = new ACClassInfoWithItems.CheckHandler()
            {
                QueryRightsFromDB = true,
                IsCheckboxVisible = true,
                CheckedSetter = null,
                CheckedGetter = null,
                CheckIsEnabledGetter = null,
            };
            ProjectManager.LoadACProject(project.ACProjectID, projectTreePresentationMode, projectTreeVisibilityFilter, projectTreeCheckHandler);
            ProjectManager.CurrentProjectItemRoot.CallOnAllItems((ACClassInfoWithItems item) => item.IsChecked = (acIdentifers == null || acIdentifers.Contains(item.ACIdentifier)));
        }
        #endregion
    }
}
