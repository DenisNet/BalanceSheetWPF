using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BalanceSheet.VerificationPass
{
    class Password
    {
        public enum Starke { низкий = 1, средний, высокий };

        public static Starke PasswordStark(string password)
        {
            int score = 0;
            Dictionary<string, int> simbols = new Dictionary<string, int> { { @"\d", 5 }, //цифры
                                                                         { @"[a-zA-Z]", 10 }, //буквы
                                                                         { @"[!,@,#,\$,%,\^,&,\*,?,_,~]", 15 } }; //символы
            if (password.Length > 8)
                foreach (var item in simbols)
                    score += Regex.Matches(password, item.Key).Count * item.Value;

            Starke result;
            switch (score / 50)
            {
                case 0: result = Starke.низкий; break;
                case 1: result = Starke.средний; break;
                default: result = Starke.высокий; break;
            }
            return result;
        }
    }
}
