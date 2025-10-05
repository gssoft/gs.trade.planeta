using System.Runtime.InteropServices;
using System.Threading;
using GS.Elements;
using GS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Tests
{
    public class A : Element1<string>
    {
        public override string Key
        {
            // get { return GetType().FullName + "@" + FullCode; }
            get
            {
                //return (Parent != null ? Parent.Key : "") +
                //                "@" + GetType().FullName + "@" + Code;
                return string.Join("@", Parent != null ? Parent.FullCode : "", FullCode);
            }
        }
    }
    public class B : Element1<string>
    {
        public A A { get; set; }

        public B()
        {
            //A = new A
            //{
            //    Code = null,
            //    Name = null,
            //    Parent = this
            //};
        }

        public override string Key
        {
            get
            {
                //return (Parent != null ? Parent.Key : "") +
                //                "@" + GetType().FullName + "@" + Code;

                return string.Join("@", Parent != null ? Parent.FullCode : "", FullCode);
            }
        }
    }
    public class C : Element1<string>
    {
        public B B { get; set; }

        public C()
        {
            //B = new B
            //{
            //    Code = "A",
            //    Name = "A",
            //    Parent = this
            //};
        }
        public override string Key
        {
            // get { return GetType().FullName + "@" + FullCode; }

            get
            {
                //return ( Parent != null ? Parent.Key : "" ) + 
                //                "@" + GetType().FullName + "@" + Code;

                return string.Join("@", Parent != null ? Parent.FullCode : "", FullCode);
            }
        }
    }
}
