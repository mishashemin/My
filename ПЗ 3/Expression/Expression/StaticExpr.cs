using System;
namespace Expression
{
    public static class Fun
    {
        public const double Pi = Math.PI;
        public const double E = Math.E;
        public static Expr Log(object arg1) => new FunLog(arg1);
        public static Expr Log(object arg1, object arg2) => new FunLog(arg1, arg2);
        public static Expr Pow(object arg1, object arg2) => new FunPow(arg1, arg2);
        public static Expr Sin(object arg) => new FunSin(arg);
        public static Expr Cos(object arg) => new FunCos(arg);
        public static Expr Tan(object arg) => new FunTan(arg);
        public static Expr Ctan(object arg) => new FunCtan(arg);
        public static Expr Asin(object arg) => new FunASin(arg);
        public static Expr Acos(object arg) => new FunACos(arg);
        public static Expr Atan(object arg) => new FunATan(arg);
        public static Expr Actan(object arg) => new FunACtan(arg);
        public static Expr Sh(object arg) => new FunSh(arg);
        public static Expr Ch(object arg) => new FunCh(arg);
        public static Expr Th(object arg) => new FunTh(arg);
        public static Expr Cth(object arg) => new FunCth(arg);       
    }
}