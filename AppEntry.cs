using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dodger
{
    public class AppEntry
    {
        static void Main()
        {
            using (DodgerGame frm = new DodgerGame())
            {
                frm.InitializeGraphics();
                System.Windows.Forms.Application.Run(frm);

                // Запись результатов в системный реестр
                frm.SaveHighScores();
            }
        }
    }
}
