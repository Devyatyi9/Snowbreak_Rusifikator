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
        //Codeberg
        public const string codebergRepoLink = "https://codeberg.org/api/v1/repos/Devyatyi9/Snowbreak_RU_translation/contents/";
        //GitLab
        public const string gitLabRepoLink = "https://gitlab.com/api/v4/projects/55335200/repository/tree/";
        //
        public const string testerBranch = "?ref=test"; //public static string mainBranch = "?ref=main";

        public static string[] arrayLinks = [gitHubRepoLink, codebergRepoLink, gitLabRepoLink];
    }
}
