using Markdig;

namespace MarkdownExtensions
{
	public class ValidationContext
	{
		public ValidationContext(RenderSettings renderSettings, MarkdownPipeline pipeline)
		{
			RenderSettings = renderSettings;
			Pipeline = pipeline;
		}
		public RenderSettings RenderSettings { get; }
		public MarkdownPipeline Pipeline { get; }
	}

	/// <summary>
	/// Enables validation of a model produced by the syntax
	/// </summary>
	public interface IValidator
    {
        IErrors Validate(object tree, ValidationContext context);
    }

	public interface IValidator<TModel> : IValidator
		where TModel : class
	{
		IErrors ValidateTyped(TModel tree, ValidationContext context);
	}

	public abstract class ValidatorBase<TModel> : IValidator<TModel>
		where TModel : class
	{
		public IErrors Validate(object tree, ValidationContext context)
		{
			return ValidateTyped((TModel)tree, context);
		}

		public abstract IErrors ValidateTyped(TModel tree, ValidationContext context);
	}
}
