namespace MarkdownExtensions
{
	/// <summary>
	/// Enables validation of a model produced by the syntax
	/// </summary>
	public interface IValidator
    {
        IErrors Validate(object tree, RenderSettings renderSettings);
    }

	public interface IValidator<TModel> : IValidator
		where TModel : class
	{
		IErrors ValidateTyped(TModel tree, RenderSettings renderSettings);
	}

	public abstract class ValidatorBase<TModel> : IValidator<TModel>
		where TModel : class
	{
		public IErrors Validate(object tree, RenderSettings renderSettings)
		{
			return ValidateTyped((TModel)tree, renderSettings);
		}

		public abstract IErrors ValidateTyped(TModel tree, RenderSettings renderSettings);
	}
}
