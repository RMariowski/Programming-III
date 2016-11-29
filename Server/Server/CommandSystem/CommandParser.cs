using System.Linq;

namespace Server
{
    /// <summary>
    /// Parser komend
    /// </summary>
    public static class CommandParser
    {
        /// <summary>
        /// Parsuje string komendy
        /// </summary>
        /// <param name="commandString">String komendy do zparsowania</param>
        /// <returns>Zparsowana komenda</returns>
        public static ICommand Parse(string commandString)
        {
            var commandParts = commandString.Split(' ').ToList();
            var commandName = commandParts[0];
            var args = commandParts.Skip(1).ToList();

            switch (commandName.ToLower())
            {
                case "exit":
                    return new ExitCommand();
            }

            return null;
        }
    }
}