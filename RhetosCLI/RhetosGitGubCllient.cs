using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhetosCLI
{
    public static class RhetosGitGubCllient
    {
        private const string PRODUCT_NAME = "Rhetos_CLI";
        private const string OWNER = "Rhetos";
        private const string REPOSITORY = "Rhetos";
        private static GitHubClient RhetosGitClient;

        public static GitHubClient GetRhetosGitClient()
        {
            if (RhetosGitClient == null)
            {
                CreateRhetosGitClient();
            }
            return RhetosGitClient;
        }

        private static void CreateRhetosGitClient()
        {
            RhetosGitClient = new GitHubClient(new ProductHeaderValue(PRODUCT_NAME));
        }

        public static async Task<List<Release>> GetAllReleases()
        {
            var github = RhetosGitGubCllient.GetRhetosGitClient();
            var releases = await github.Repository.Release.GetAll(OWNER, REPOSITORY);

            return releases.ToList();
        }
    }
}