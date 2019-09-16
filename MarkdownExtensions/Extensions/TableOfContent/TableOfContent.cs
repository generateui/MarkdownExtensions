using System.Collections.Generic;

namespace MarkdownExtensions.Extensions.TableOfContent
{
	public class TableOfContent
	{
		public NumberingStyle Level1 { get; set; } = NumberingStyle.None;
		public NumberingStyle Level2 { get; set; } = NumberingStyle.Decimal;
		public NumberingStyle Level3 { get; set; } = NumberingStyle.Decimal;
		public NumberingStyle Level4 { get; set; } = NumberingStyle.Decimal;
		public NumberingStyle Level5 { get; set; } = NumberingStyle.Decimal;
		public NumberingStyle Level6 { get; set; } = NumberingStyle.Decimal;

		internal static TableOfContent Empty() => new TableOfContent
		{
			Level1 = NumberingStyle.None,
			Level2 = NumberingStyle.None,
			Level3 = NumberingStyle.None,
			Level4 = NumberingStyle.None,
			Level5 = NumberingStyle.None,
			Level6 = NumberingStyle.None,
		};
	}
	// not supported: circle, square, disc, disclosure-open, disclosure-closed
	public class NumberingStyle
	{
		private static Dictionary<string, NumberingStyle> _byName;

		public NumberingStyle(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public static NumberingStyle None = new NumberingStyle("none");

		public static NumberingStyle ArabicIndic = new NumberingStyle("arabic-indic");
		public static NumberingStyle Armenian = new NumberingStyle("armenian");
		public static NumberingStyle Bengali = new NumberingStyle("bengali");
		public static NumberingStyle Cambodian = new NumberingStyle("cambodian");
		public static NumberingStyle CjkDecimal = new NumberingStyle("cjk-decimal");
		public static NumberingStyle CjkEarthlyBranch = new NumberingStyle("cjk-earthly-branch");
		public static NumberingStyle CjkHeavenlyStem = new NumberingStyle("cjk-heavenly-stem");
		public static NumberingStyle Decimal = new NumberingStyle("decimal");
		public static NumberingStyle DecimalLeadingZero = new NumberingStyle("decimal-leading-zero");
		public static NumberingStyle Devanagari = new NumberingStyle("devanagari");
		public static NumberingStyle EthiopicNumeric = new NumberingStyle("ethiopic-numeric");
		public static NumberingStyle Georgian = new NumberingStyle("georgian");
		public static NumberingStyle Gujarati = new NumberingStyle("gujarati");
		public static NumberingStyle Gurmukhi = new NumberingStyle("gurmukhi");
		public static NumberingStyle Hebrew = new NumberingStyle("hebrew");
		public static NumberingStyle Hiragana = new NumberingStyle("hiragana");
		public static NumberingStyle HiraganaIroha = new NumberingStyle("hiragana-iroha");
		public static NumberingStyle JapaneseFormal = new NumberingStyle("japanese-formal");
		public static NumberingStyle JapaneseInformal = new NumberingStyle("japanese-informal");
		public static NumberingStyle Kannada = new NumberingStyle("kannada");
		public static NumberingStyle Katakana = new NumberingStyle("katakana");
		public static NumberingStyle KatakanaIrhoa = new NumberingStyle("katakana-iroha");
		public static NumberingStyle Khmer = new NumberingStyle("khmer");
		public static NumberingStyle KoreanHangulFormal = new NumberingStyle("korean-hangul-formal");
		public static NumberingStyle KoreanHanjaFormal = new NumberingStyle("korean-hanja-formal");
		public static NumberingStyle KoreanHanjaInformal = new NumberingStyle("korean-hanja-informal");
		public static NumberingStyle Lao = new NumberingStyle("lao");
		public static NumberingStyle LowerAlpha = new NumberingStyle("lower-alpha");
		public static NumberingStyle LowerArmenian = new NumberingStyle("lower-armenian");
		public static NumberingStyle LowerGreek = new NumberingStyle("lower-greek");
		public static NumberingStyle LowerLatin = new NumberingStyle("lower-latin");
		public static NumberingStyle LowerRoman = new NumberingStyle("lower-roman");
		public static NumberingStyle Malayalam = new NumberingStyle("malayalam");
		public static NumberingStyle Nongolian = new NumberingStyle("mongolian");
		public static NumberingStyle Nyanmar = new NumberingStyle("myanmar");
		public static NumberingStyle Oriya = new NumberingStyle("oriya");
		public static NumberingStyle Persian = new NumberingStyle("persian");
		public static NumberingStyle SimplifiedChineseFormal = new NumberingStyle("simp-chinese-formal");
		public static NumberingStyle SimplifiedChineseInformal = new NumberingStyle("simp-chinese-informal");
		public static NumberingStyle Tamil = new NumberingStyle("tamil");
		public static NumberingStyle Telugu = new NumberingStyle("telugu");
		public static NumberingStyle Thai = new NumberingStyle("thai");
		public static NumberingStyle Tibetan = new NumberingStyle("tibetan");
		public static NumberingStyle TraditionalChineseFormal = new NumberingStyle("trad-chinese-formal");
		public static NumberingStyle TraditionalChineseInformal = new NumberingStyle("trad-chinese-informal");
		public static NumberingStyle UpperAlpha = new NumberingStyle("upper-alpha");
		public static NumberingStyle UpperArmenian = new NumberingStyle("upper-armenian");
		public static NumberingStyle UpperLatin = new NumberingStyle("upper-latin");
		public static NumberingStyle UpperRoman = new NumberingStyle("upper-roman");

		public override bool Equals(object obj) =>
			obj is NumberingStyle style && Name == style.Name;

		public override int GetHashCode() =>
			539060726 + EqualityComparer<string>.Default.GetHashCode(Name);

		private static void EnsureDictionaryIsInitialized()
		{
			if (_byName != null)
			{
				return;
			}
			_byName = new Dictionary<string, NumberingStyle>
			{
				[ArabicIndic.Name] = ArabicIndic,
				[Armenian.Name] = Armenian,
				[Bengali.Name] = Bengali,
				[Cambodian.Name] = Cambodian,
				[CjkDecimal.Name] = CjkDecimal,
				[CjkEarthlyBranch.Name] = CjkEarthlyBranch,
				[CjkHeavenlyStem.Name] = CjkHeavenlyStem,
				[Decimal.Name] = Decimal,
				[DecimalLeadingZero.Name] = DecimalLeadingZero,
				[Devanagari.Name] = Devanagari,
				[EthiopicNumeric.Name] = EthiopicNumeric,
				[Georgian.Name] = Georgian,
				[Gujarati.Name] = Gujarati,
				[Gurmukhi.Name] = Gurmukhi,
				[Hebrew.Name] = Hebrew,
				[Hiragana.Name] = Hiragana,
				[HiraganaIroha.Name] = HiraganaIroha,
				[JapaneseFormal.Name] = JapaneseFormal,
				[JapaneseInformal.Name] = JapaneseInformal,
				[Kannada.Name] = Kannada,
				[Katakana.Name] = Katakana,
				[KatakanaIrhoa.Name] = KatakanaIrhoa,
				[Khmer.Name] = Khmer,
				[KoreanHangulFormal.Name] = KoreanHangulFormal,
				[KoreanHanjaFormal.Name] = KoreanHanjaFormal,
				[KoreanHanjaInformal.Name] = KoreanHanjaInformal,
				[Lao.Name] = Lao,
				[LowerAlpha.Name] = LowerAlpha,
				[LowerArmenian.Name] = LowerArmenian,
				[LowerGreek.Name] = LowerGreek,
				[LowerLatin.Name] = LowerLatin,
				[LowerRoman.Name] = LowerRoman,
				[Malayalam.Name] = Malayalam,
				[Nongolian.Name] = Nongolian,
				[Nyanmar.Name] = Nyanmar,
				[Oriya.Name] = Oriya,
				[Persian.Name] = Persian,
				[SimplifiedChineseFormal.Name] = SimplifiedChineseFormal,
				[SimplifiedChineseInformal.Name] = SimplifiedChineseInformal,
				[Tamil.Name] = Tamil,
				[Telugu.Name] = Telugu,
				[Thai.Name] = Thai,
				[Tibetan.Name] = Tibetan,
				[TraditionalChineseFormal.Name] = TraditionalChineseFormal,
				[TraditionalChineseInformal.Name] = TraditionalChineseInformal,
				[UpperAlpha.Name] = UpperAlpha,
				[UpperArmenian.Name] = UpperArmenian,
				[UpperLatin.Name] = UpperLatin,
				[UpperRoman.Name] = UpperRoman
			};
		}

		public static NumberingStyle TryParse(string name)
		{
			EnsureDictionaryIsInitialized();
			var sanitized = name.Trim().ToLower();
			if (_byName.ContainsKey(sanitized))
			{
				return _byName[sanitized];
			}
			return null;
		}
	}
}
