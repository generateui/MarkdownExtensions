using EA;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using SimpleInjector;
using System;

namespace MarkdownExtension.EnterpriseArchitect
{
    public static class Plugin
    {
        /// <summary>
        /// Wraps the EA repository instance to enable the DI container to release its resources
        /// </summary>
        public class RepositoryWrapper : IDisposable
        {
            private RepositoryClass _repository;
			private readonly EnterpriseArchitectSettings _settings;

			public RepositoryWrapper(EnterpriseArchitectSettings settings)
			{
				_settings = settings;
			}

            public RepositoryClass Repository
            {
                get
                {
                    if (_repository == null)
                    {
                        _repository = new RepositoryClass();
                        try
                        {
							//var folder = Assembly.GetExecutingAssembly().CodeBase;
							//folder = System.IO.Path.GetDirectoryName(folder);
							//var file = "EaTest.eapx";
							//var filePath = System.IO.Path.Combine(folder, file);
							//_repository.OpenFile(new Uri(filePath).AbsolutePath);
							_repository.OpenFile(_settings.File.AbsolutePath);
						}
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                    return _repository;
                }
            }

            public void Dispose()
            {
				if (_repository != null)
				{
					Repository.CloseFile();
					Repository.Exit();
				}
			}

            public static implicit operator RepositoryClass(RepositoryWrapper wrapper)
            {
                return wrapper.Repository;
            }
        }

        public static void Register(Container container)
        {
            container.Register<RepositoryWrapper>(Lifestyle.Scoped);
            container.Register<IEaProvider, CacheProvider>(Lifestyle.Scoped);
            container.Register<EaProvider.EaProvider>(Lifestyle.Scoped);
            container.Register<JsonProvider>(Lifestyle.Scoped);
			container.Register<EnterpriseArchitectSettings>(Lifestyle.Singleton);

			container.Collection.Append<IExtensionSettings, EnterpriseArchitectSettings>(Lifestyle.Singleton);
        }
    }
}
