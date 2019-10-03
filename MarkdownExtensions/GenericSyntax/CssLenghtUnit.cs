using System;
using System.Collections.Generic;

namespace MarkdownExtensions.GenericSyntax
{
	// https://developer.mozilla.org/en-US/docs/Web/CSS/length
	public enum CssLength
	{
		// Relative length units
		cap, ch, em, ex, ic, lh, rem, rlh,
		// viewport-percentage lengths
		vh, vw, vi, vb, vmin, vmax,
		// absolute length units
		px, cm, mm, Q, @in, pc, pt,
	}
	public sealed class CssLenghtUnit
	{
		private static readonly Dictionary<string, CssLength> _unitByName = new Dictionary<string, CssLength>
		{
			["cap"] = CssLength.cap,
			["ch"] = CssLength.ch,
			["em"] = CssLength.em,
			["ex"] = CssLength.ex,
			["ic"] = CssLength.ic,
			["lh"] = CssLength.lh,
			["rem"] = CssLength.rem,
			["rlh"] = CssLength.rlh,
			["vh"] = CssLength.vh,
			["vw"] = CssLength.vw,
			["vi"] = CssLength.vi,
			["vb"] = CssLength.vb,
			["vmin"] = CssLength.vmin,
			["vmax"] = CssLength.vmax,
			["px"] = CssLength.px,
			["cm"] = CssLength.cm,
			["mm"] = CssLength.mm,
			["Q"] = CssLength.Q,
			["in"] = CssLength.@in,
			["pc"] = CssLength.pc,
			["pt"] = CssLength.pt,
		};


		private string _text;

		public CssLenghtUnit(float value, CssLength unit, string text = null)
		{
			Value = value;
			Unit = unit;
			_text = text;
		}

		public float Value { get; }
		public CssLength Unit { get; }

		private enum State
		{
			NumberBeforeDot,
			NumberAfterDot,
			NumberAfterExponent,
			Unit
		}
		// unique characters used in length units: a, b, c, d, e, h, i, l, m, n, p, Q, r, t, v, w, x
		public static CssLenghtUnit Parse(string text)
		{
			var state = State.NumberBeforeDot;
			string number = "";
			string unitName = "";
			foreach (char c in text)
			{
				if (state == State.NumberBeforeDot)
				{
					// e E
					if (c == '-' || c == '+' || char.IsDigit(c))
					{
						number += c;
						continue;
					}
					if (c == '.')
					{
						number += c;
						state = State.NumberAfterDot;
						continue;
					}
				}
				if (state == State.NumberAfterDot)
				{
					if (char.IsDigit(c))
					{
						number += c;
						continue;
					}
					if (c == 'e' || c == 'E')
					{
						number += c;
						state = State.NumberAfterExponent;
						continue;
					}
				}
				if (state == State.NumberAfterExponent)
				{
					if (char.IsDigit(c))
					{
						number += c;
						continue;
					}
				}
				if (c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' ||
					c == 'h' || c == 'i' || c == 'l' || c == 'm' || c == 'n' ||
					c == 'p' || c == 'Q' || c == 'r' || c == 't' || c == 'v' ||
					c == 'w' || c == 'x')
				{
					if (state != State.Unit)
					{
						state = State.Unit;
					}
					unitName += c;
					continue;
				}
				throw new ArgumentException("");
			}
			if (!float.TryParse(number, out float value))
			{
				throw new ArgumentException("incorrect number"); // TODO
			}
			if (!_unitByName.ContainsKey(unitName))
			{
				throw new ArgumentException("incorrect unit"); // TODO
			}
			CssLength unit = _unitByName[unitName];
			return new CssLenghtUnit(value, unit, text);
		}

		public override bool Equals(object obj) => 
			obj is CssLenghtUnit unit &&
			Value == unit.Value &&
			Unit == unit.Unit;

		public override int GetHashCode()
		{
			var hashCode = -177567199;
			hashCode = hashCode * -1521134295 + Value.GetHashCode();
			hashCode = hashCode * -1521134295 + Unit.GetHashCode();
			return hashCode;
		}

		public override string ToString()
		{
			if (_text != null)
			{
				return _text;
			}
			return base.ToString();
		}
	}
}
