
using Server.Accounting;
using Server.Mobiles;
using System.Collections;

namespace Server.Commands
{
    public class DotCommand_AccountWipe
    {
        public static void Initialize()
        {
            CommandSystem.Register("AccountWipe", AccessLevel.Owner, new CommandEventHandler(OnCommand_AccountWipe));
        }
        
        private static void OnCommand_AccountWipe(CommandEventArgs e)
        {
            ArrayList account = new ArrayList();
            ArrayList mobs = new ArrayList(World.Mobiles.Values);
            Account acct;
            ArrayList pmsfnd = new ArrayList();

            foreach (Mobile m in mobs)
            {
                if (m.Account != null)
                {
                    acct = m.Account as Account;
                    if (m.Account.AccessLevel == AccessLevel.Player)
                    {
                        m.Delete();
                    }

                }
            }
            

            foreach (Account accounts in Accounts.GetAccounts())
            {
                if (accounts.AccessLevel == AccessLevel.Player)
                    account.Add(accounts);
            }

            if (account != null)
            {
                foreach (Account ac in account)

                    ac.Delete();
            }

            e.Mobile.SendMessage(MessageUtil.MessageColorGM, "Accounts deleted");
        }
    }
}