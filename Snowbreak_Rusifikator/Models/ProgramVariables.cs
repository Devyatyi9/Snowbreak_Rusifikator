using Snowbreak_Rusifikator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator
{
    public static class ProgramVariables
    {
        //GitHub
        public const string gitHubRepoLink = "https://api.github.com/repos/Devyatyi9/Snowbreak_RU_translation/contents/";
        public const string testerBranch = "?ref=test"; //public static string mainBranch = "?ref=main";
        public const string gitHubToken = GitHubToken.token;
        //Codeberg
        public const string codebergRepoLink = "";
        //public const string gitLabToken = "";
        public const string codebergTesterBranch = "?ref=test";
    }
}
