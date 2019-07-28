namespace MarkdownExtensions
{
	public interface IValidator
    {
        IErrors Validate(object tree, SourceSettings sourceSettings);
    }

	public interface IValidator<TModel> : IValidator
		where TModel : class
	{
		IErrors ValidateTyped(TModel tree, SourceSettings sourceSettings);
	}

	public abstract class ValidatorBase<TModel> : IValidator<TModel>
		where TModel : class
	{
		public IErrors Validate(object tree, SourceSettings sourceSettings)
		{
			return ValidateTyped((TModel)tree, sourceSettings);
		}

		public abstract IErrors ValidateTyped(TModel tree, SourceSettings sourceSettings);
	}
}
