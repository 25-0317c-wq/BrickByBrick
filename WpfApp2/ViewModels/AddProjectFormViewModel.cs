using System.Collections.ObjectModel;
using BrickByBrick.Models;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Backs the "Add Project" modal — lets Admin/Manager create a project
    /// directly (bypassing the Client-submission flow), matching the fields
    /// from the Add Project wireframe: name, phase, location, client,
    /// project type, and building category.
    /// </summary>
    public class AddProjectFormViewModel : BaseViewModel
    {
        private string _projectName = string.Empty;
        public string ProjectName
        {
            get => _projectName;
            set { _projectName = value; OnPropertyChanged(); }
        }

        private string _projectPhase = string.Empty;
        public string ProjectPhase
        {
            get => _projectPhase;
            set { _projectPhase = value; OnPropertyChanged(); }
        }

        private string _projectLocation = string.Empty;
        public string ProjectLocation
        {
            get => _projectLocation;
            set { _projectLocation = value; OnPropertyChanged(); }
        }

        private string _client = string.Empty;
        public string Client
        {
            get => _client;
            set { _client = value; OnPropertyChanged(); }
        }

        private ProjectType _projectType = ProjectType.Commercial;
        public ProjectType ProjectType
        {
            get => _projectType;
            set { _projectType = value; OnPropertyChanged(); }
        }

        private BuildingCategory _buildingCategory = BuildingCategory.Office;
        public BuildingCategory BuildingCategory
        {
            get => _buildingCategory;
            set { _buildingCategory = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ProjectType> ProjectTypeOptions { get; } =
            new ObservableCollection<ProjectType>((ProjectType[])System.Enum.GetValues(typeof(ProjectType)));

        public ObservableCollection<BuildingCategory> BuildingCategoryOptions { get; } =
            new ObservableCollection<BuildingCategory>((BuildingCategory[])System.Enum.GetValues(typeof(BuildingCategory)));

        public void Reset()
        {
            ProjectName = string.Empty;
            ProjectPhase = string.Empty;
            ProjectLocation = string.Empty;
            Client = string.Empty;
            ProjectType = ProjectType.Commercial;
            BuildingCategory = BuildingCategory.Office;
        }
    }
}
