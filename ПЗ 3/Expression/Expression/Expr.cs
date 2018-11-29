using System;
using System.Collections.Generic;

namespace Expression
{
    using static Fun;
    public abstract class Expr
    {
        public abstract double Compute(IReadOnlyDictionary<string, double> variableValues = null);
        public abstract bool IsConstant { get; protected set; }
        public abstract Expr Diff();      
        public static Expr operator +(Expr Operand) => new Plus(Operand);
        public static Expr operator -(Expr Operand) => new Minus(Operand);

        #region "operator +"
        public static Expr operator +(Expr Operand1, Expr Operand2) => new Sum(Operand1, Operand2);
        public static Expr operator +(Expr Operand1, double Operand2) => new Sum(Operand1, new Constant(Operand2));
        public static Expr operator +(double Operand1, Expr Operand2) => new Sum(new Constant(Operand1), Operand2);
        #endregion

        #region "operator -"
        public static Expr operator -(Expr Operand1, Expr Operand2) => new Sub(Operand1, Operand2);
        public static Expr operator -(Expr Operand1, double Operand2) => new Sub(Operand1, new Constant(Operand2));
        public static Expr operator -(double Operand1, Expr Operand2) => new Sub(new Constant(Operand1), Operand2);

        #endregion

        #region "operator /"
        public static Expr operator /(Expr Operand1, Expr Operand2) => new Div(Operand1, Operand2);
        public static Expr operator /(Expr Operand1, double Operand2) => new Div(Operand1, new Constant(Operand2));
        public static Expr operator /(double Operand1, Expr Operand2) => new Div(new Constant(Operand1), Operand2);

        #endregion

        #region "operator *"
        public static Expr operator *(Expr Operand1, Expr Operand2) => new Mult(Operand1, Operand2);
        public static Expr operator *(Expr Operand1, double Operand2) => new Mult(Operand1, new Constant(Operand2));
        public static Expr operator *(double Operand1, Expr Operand2) => new Mult(new Constant(Operand1), Operand2);

        #endregion

    }
    public class Constant : Expr
    {
        public override bool IsConstant { get => true; protected set => IsConstant = value; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Const;
        public override Expr Diff() => new Constant(0);
        public double Const { get; }
        public Constant(object Constant) => Const = Constant is double ? (double)Constant : Constant is int ? (int)Constant : 0;
        public override string ToString() => Const.ToString();       
    }
    public class Variable : Expr
    {
        public override bool IsConstant { get => false; protected set => IsConstant = value; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues)
        {
            if (variableValues != null)
                if (variableValues.ContainsKey(Var)) return variableValues[Var];
            return 0;
        }
        public override Expr Diff() => new Constant(1);
        public string Var { get; }
        public Variable(string var)
        {
            Var = var;
        }
        public override string ToString() => Var;
    }

    public abstract class UnaryOperation : Expr
    {
        public abstract Expr Operand { get; protected set; }
        protected UnaryOperation(Expr operand)
        {
            Operand = operand;
            IsConstant = operand.IsConstant ? true : false;
        }
    }

    public abstract class BinaryOperation : Expr
    {
        public abstract Expr Operand1 { get; protected set; }
        public abstract Expr Operand2 { get; protected set; }
        protected BinaryOperation(Expr operand1, Expr operand2)
        {
            Operand1 = operand1;
            Operand2 = operand2;
            IsConstant = (Operand1.IsConstant && Operand2.IsConstant) ? true : false;
        }
    }

    public abstract class Function : Expr
    {
        public abstract Expr Arg1 { get; protected set; }
        protected Function(object arg)
        {
            Arg1 = arg as Expr;
            IsConstant = Arg1 == null ? true : false;
            Arg1 = Arg1 ?? new Constant(arg);
        }
        protected Function()
        { }
    }

    #region UnaryOperation
    class Minus : UnaryOperation
    {
        public override bool IsConstant { get; protected set; } = false;
        public override Expr Operand { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => -Operand.Compute(variableValues);
        public override Expr Diff() => !IsConstant ? -Operand.Diff() : new Constant(0);
        public Minus(Expr Operand) : base(Operand) { }
        public override string ToString() => "-" + Operand.ToString();
    }

    class Plus : UnaryOperation
    {
        public override bool IsConstant { get; protected set; } = false;
        public override Expr Operand { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Operand.Compute(variableValues);
        public override Expr Diff() => !IsConstant ? Operand.Diff() : new Constant(0);
        public Plus(Expr Operand) : base(Operand) { }
        public override string ToString() => "+" + Operand.ToString();
    }
    #endregion

    #region BinaryOperation
    class Sum : BinaryOperation
    {
        public override bool IsConstant { get; protected set; } = false;
        public override Expr Operand1 { get; protected set; }
        public override Expr Operand2 { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Operand1.Compute(variableValues) + Operand2.Compute(variableValues);
        public override Expr Diff() => !IsConstant ? Operand1.Diff() + Operand2.Diff() : new Constant(0);
        public Sum(Expr Operand1, Expr Operand2) : base(Operand1, Operand2) { }
        public override string ToString() => "( " + Operand1.ToString() + " + " + Operand2.ToString() + " )";
    }

    class Sub : BinaryOperation
    {
        public override bool IsConstant { get; protected set; } = false;
        public override Expr Operand1 { get; protected set; }
        public override Expr Operand2 { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Operand1.Compute(variableValues) - Operand2.Compute(variableValues);
        public override Expr Diff() => !IsConstant ? Operand1.Diff() - Operand2.Diff() : new Constant(0);
        public Sub(Expr Operand1, Expr Operand2) : base(Operand1, Operand2) { }
        public override string ToString() => "( " + Operand1.ToString() + " - " + Operand2.ToString() + " )";
    }

    class Div : BinaryOperation
    {
        public override bool IsConstant { get; protected set; } = false;
        public override Expr Operand1 { get; protected set; }
        public override Expr Operand2 { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Operand1.Compute(variableValues) / Operand2.Compute(variableValues);
        public override Expr Diff() => !IsConstant ? (Operand1.Diff() * Operand2 - Operand2.Diff() * Operand1) / Pow(Operand2, 2) : new Constant(0);
        public Div(Expr Operand1, Expr Operand2) : base(Operand1, Operand2) { }
        public override string ToString() => "( " + Operand1.ToString() + " / " + Operand2.ToString() + " )";
    }

    class Mult : BinaryOperation
    {
        public override bool IsConstant { get; protected set; } = false;
        public override Expr Operand1 { get; protected set; }
        public override Expr Operand2 { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Operand1.Compute(variableValues) * Operand2.Compute(variableValues);
        public override Expr Diff() => !IsConstant ? Operand1.Diff() * Operand2 + Operand2.Diff() * Operand1 : new Constant(0);
        public Mult(Expr Operand1, Expr Operand2) : base(Operand1, Operand2) { }
        public override string ToString() => "( " + Operand1.ToString() + " * " + Operand2.ToString() + " )";
    }
    #endregion   
    class FunPow : Function
    {
        public override Expr Arg1 { get; protected set; } = null;
        public Expr Arg2 { get; } = null;
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Pow(Arg1.Compute(variableValues), Arg2.Compute(variableValues));
        public override Expr Diff()
        {
            if (!IsConstant)
            {
                if (Arg1.IsConstant)
                {
                    if (Arg1.Compute() == E) return new FunPow(Arg1, Arg2);
                    else return Pow(Arg1, Arg2) * Log(Arg1);
                }
                else if (Arg2.IsConstant)
                {
                    return Arg2 * Pow(Arg1, Arg2 - 1);
                }
                throw new NotImplementedException();
            }
            else return new Constant(0);

        }
        public FunPow(object arg1, object arg2)
        {
            Arg1 = arg1 as Expr;
            Arg2 = arg2 as Expr;
            IsConstant = (Arg1 == null && Arg2 == null) ? true : false;
            Arg1 = Arg1 ?? new Constant(arg1);
            Arg2 = Arg2 ?? new Constant(arg2);
        }
        public override string ToString() => "( " + Arg1.ToString() + " ^ " + Arg2.ToString() + " )";
    }
    class FunLog : Function
    {
        public override Expr Arg1 { get; protected set; } = null;
        public Expr Arg2 { get; } = null;
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues)
        {
            return Arg2 == null ? Math.Log(Arg1.Compute(variableValues)) : Math.Log(Arg1.Compute(variableValues), Arg2.Compute(variableValues));
        }
        public override Expr Diff()
        {
            if (!IsConstant)
            {
                if (Arg2 == null) return 1 / Arg1;
                else if (Arg2.IsConstant) return 1 / (Arg1 * Log(Arg1));
                throw new NotImplementedException();
            }
            else return new Constant(0);
        }
        public FunLog(object arg1, object arg2)
        {
            Arg1 = arg1 as Expr;
            Arg2 = arg2 as Expr;
            IsConstant = (Arg1 == null && Arg2 == null) ? true : false;
            Arg1 = Arg1 ?? new Constant(arg1);
            Arg2 = Arg2 ?? new Constant(arg2);
        }
        public FunLog(object arg1)
        {
            Arg1 = arg1 as Expr;
            IsConstant = Arg1 == null ? true : false;
            Arg1 = Arg1 ?? new Constant(arg1);
        }
        public override string ToString()
        {
            if (Arg2 == null) return "Log(" + "" + Arg1.ToString() + ")";
            else return "Log(" + "" + Arg1.ToString() + "," + Arg2.ToString() + ")";
        }
    }
    #region Trigonometry Function
    class FunSin : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues)
        {
            double arg = Arg1.Compute(variableValues);
            double Radian = arg * Math.PI / 180;
            return Math.Sin(Radian);
        }
        public override Expr Diff() => !IsConstant ? Cos(Arg1) * Arg1.Diff() : new Constant(0);
        public FunSin(object arg) : base(arg) { }
        public override string ToString() => "Sin(" + Arg1.ToString() + ")";
    }
    class FunCos : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues)
        {
            double arg = Arg1.Compute(variableValues);
            double Radian = arg * Math.PI / 180;
            return Math.Cos(Radian);
        }
        public override Expr Diff() => !IsConstant ? -Sin(Arg1) * Arg1.Diff() : new Constant(0);
        public FunCos(object arg) : base(arg) { }
        public override string ToString() => "Cos(" + Arg1.ToString() + ")";
    }
    class FunTan : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues)
        {
            double arg = Arg1.Compute(variableValues);
            double Radian = arg * Math.PI / 180;
            return Math.Tan(Radian);
        }
        public override Expr Diff() => !IsConstant ? (1 / Pow(Cos(Arg1), 2)) * Arg1.Diff() : new Constant(0);
        public FunTan(object arg) : base(arg) { }
        public override string ToString() => "Tan(" + Arg1.ToString() + ")";
    }
    class FunCtan : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues)
        {
            double arg = Arg1.Compute(variableValues);
            double Radian = arg * Math.PI / 180;
            return 1 / Math.Tan(Radian);
        }
        public override Expr Diff() => !IsConstant ? -(1 / Pow(Sin(Arg1), 2)) * Arg1.Diff() : new Constant(0);
        public FunCtan(object arg) : base(arg) { }
        public override string ToString() => "Ctan(" + Arg1.ToString() + ")";
    }
    class FunASin : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Asin(Arg1.Compute(variableValues)) * 180 / Math.PI;
        public override Expr Diff() => !IsConstant ? (1 / Pow(1 - Pow(Arg1, 2), 0.5)) * Arg1.Diff() : new Constant(0);        
        public FunASin(object arg) : base(arg) { }
        public override string ToString() => "Asin(" + Arg1.ToString() + ")";
    }
    class FunACos : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Acos(Arg1.Compute(variableValues)) * 180 / Math.PI;       
        public override Expr Diff() => !IsConstant ? -(1 / Pow(1 - Pow(Arg1, 2), 0.5)) * Arg1.Diff() : new Constant(0);
        public FunACos(object arg) : base(arg) { }
        public override string ToString() => "Acos(" + Arg1.ToString() + ")";
    }
    class FunATan : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Atan(Arg1.Compute(variableValues)) * 180 / Math.PI;       
        public override Expr Diff() => !IsConstant? (1 / (1 + Pow(Arg1,2))) * Arg1.Diff() : new Constant(0);
        public FunATan(object arg) : base(arg) { }
        public override string ToString() => "Atan(" + Arg1.ToString() + ")";
    }
    class FunACtan : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; }
    public override double Compute(IReadOnlyDictionary<string, double> variableValues) => (Math.PI / 2 - Math.Atan(Arg1.Compute(variableValues))) * 180 / Math.PI;
        public override Expr Diff() => !IsConstant ? -(1 / (1 + Pow(Arg1, 2))) * Arg1.Diff() : new Constant(0);
        public FunACtan(object arg) : base(arg) { }
        public override string ToString() => "Actan(" + Arg1.ToString() + ")";
    }
    class FunSh : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Sinh(Arg1.Compute(variableValues));
        public override Expr Diff() => !IsConstant ? Arg1 * Arg1.Diff() : new Constant(0);
        public FunSh(object arg) : base(arg) { }
        public override string ToString() => "Sh(" + Arg1.ToString() + ")";
    }
    class FunCh : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Cosh(Arg1.Compute(variableValues));
        public override Expr Diff() => !IsConstant ? Arg1 * Arg1.Diff() : new Constant(0);
        public FunCh(object arg) : base(arg) { }
        public override string ToString() => "Ch(" + Arg1.ToString() + ")";
    }
    class FunTh : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => Math.Tanh(Arg1.Compute(variableValues));
        public override Expr Diff() => !IsConstant ? (1/Pow(Ch(Arg1),2)) * Arg1.Diff() : new Constant(0);
        public FunTh(object arg) : base(arg) { }
        public override string ToString() => "Th(" + Arg1.ToString() + ")";
    }
    class FunCth : Function
    {
        public override Expr Arg1 { get; protected set; }
        public override bool IsConstant { get; protected set; } = false;
        public override double Compute(IReadOnlyDictionary<string, double> variableValues) => 1 / Math.Tanh(Arg1.Compute(variableValues));
        public override Expr Diff() => !IsConstant ? -(1 / Pow(Ch(Arg1), 2)) * Arg1.Diff() : new Constant(0);
        public FunCth(object arg) : base(arg) { }
        public override string ToString() => "Cth(" + Arg1.ToString() + ")";
    }
    #endregion
}
