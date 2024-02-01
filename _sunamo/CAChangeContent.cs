
namespace SunamoWinStd._sunamo;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SunamoWinStd._sunamo
//{
//    internal class CAChangeContent
//    {
//        internal static List<string> ChangeContent1(ChangeContentArgs a, List<string> files_in, Func<string, string, string> func, string a1)
//        {
//            var result = ChangeContent<string>(a, files_in, func, a1);
//            return result;
//        }

//        /// <summary>
//        /// Direct edit input collection
//        ///
//        /// Dříve to bylo List<string> files_in, Func<string,
//        /// </summary>
//        /// <typeparam name="Arg1"></typeparam>
//        /// <param name="files_in"></param>
//        /// <param name="func"></param>
//        /// <param name="arg"></param>
//        internal static List<string> ChangeContent<Arg1>(ChangeContentArgs a, List<string> files_in, Func<string, Arg1, string> func, Arg1 arg, Func<Arg1, string, string> funcSwitch12 = null)
//        {
//            if (a == null)
//            {
//                a = new ChangeContentArgs();
//            }

//            if (a.switchFirstAndSecondArg)
//            {
//                files_in = ChangeContentSwitch12<Arg1>(files_in, funcSwitch12, arg);
//            }
//            else
//            {
//                for (int i = 0; i < files_in.Count; i++)
//                {
//                    files_in[i] = func.Invoke(files_in[i], arg);
//                }
//            }


//            RemoveNullOrEmpty(a, files_in);

//            return files_in;
//        }
//    }
//}
