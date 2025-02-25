using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.Gitmoji_Plus {
    public class Main : IPlugin {
        private PluginInitContext _context;

        public void Init(PluginInitContext context) {
            _context = context;
        }

        public List<Result> Query(Query query) {
            return new List<Result> {
                new Result {
                    Title = "🎉 initial gitmoji search",
                    SubTitle = "search for gitmoji hehe 😜",
                    IcoPath = "Images/icon.png"
                }
            };
        }

    }
}