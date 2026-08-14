using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static P3PPC.Expansion.TheAnswer.Anim;

namespace P3PPC.Expansion.TheAnswer
{
    internal static unsafe class Anim
    {

        [StructLayout(LayoutKind.Sequential)]
        internal struct AnimArray
        {
            internal byte ANIM;
        }

    }
}
