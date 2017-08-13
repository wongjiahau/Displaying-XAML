using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDisplayer {
    public class MaterialDesignInXamlToolkitGitHubFile : GitHubFile {
        private const string RepositoryName = "MaterialDesignInXamlToolkit";
        private const string FolderPath = "MainDemo.Wpf";

        public MaterialDesignInXamlToolkitGitHubFile(string typeName) : 
            base("wongjiahau" , RepositoryName, "New-Demo-2", FolderPath, typeName + ".xaml") {

        }
        public MaterialDesignInXamlToolkitGitHubFile(string ownerName , string branchName , string fileName)
            : base(ownerName , RepositoryName , branchName , FolderPath , fileName) { }
    }
}
