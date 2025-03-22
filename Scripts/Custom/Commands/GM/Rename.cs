
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    public class RenameCommand
    {

        private class InternalTarget : Target
        {

            private string m_newName;

            public InternalTarget(string newName)
                : base(20, true, TargetFlags.None)
            {
                m_newName = newName;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target != null)
                    if (target is Mobile)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} renaming {2} to {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(target), m_newName);
                        ((Mobile)target).Name = m_newName;
                        from.SendMessage(MessageUtil.MessageColorGM, "You renamed it to {0}.", m_newName);

                    }

                    else if (target is Item)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} renaming {2} to {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(target), m_newName);
                        ((Item)target).Name = m_newName;
                        from.SendMessage(MessageUtil.MessageColorGM, "You renamed it to {0}.", m_newName);
                    }
                    else
                        from.SendMessage(MessageUtil.MessageColorGM, "You must target a mobile or item");
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("rename", AccessLevel.GameMaster, new CommandEventHandler(EventCommand_Rename));
        }

        public static void EventCommand_Rename(CommandEventArgs e)
        {
            string newName = e.ArgString.Trim();

            if (newName.Length > 0)
            {
                e.Mobile.Target = new InternalTarget(e.ArgString);
                e.Mobile.SendMessage(MessageUtil.MessageColorGM, "Target what you would like to name this.");
            }
            else
                e.Mobile.SendMessage(MessageUtil.MessageColorGM, "Format: .rename \"<name>\"");

        }
    }
}
