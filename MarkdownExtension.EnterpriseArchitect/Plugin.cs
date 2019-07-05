using EA;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using SimpleInjector;
using System;
using System.Runtime.InteropServices;

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

            public RepositoryClass Repository
            {
                get
                {
                    if (_repository == null)
                    {
                        _repository = new RepositoryClass();
                        try
                        {
                            _repository.OpenFile(@"c:\Users\RuudP\Desktop\Ars.EAP");
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
                Repository.CloseFile();
                Marshal.ReleaseComObject(Repository);
                Repository.Exit();
            }

            public static implicit operator RepositoryClass (RepositoryWrapper wrapper)
            {
                return wrapper.Repository;
            }
        }

        public static void Register(Container container)
        {
            container.Register<Func<RepositoryWrapper>>(() => container.GetInstance<RepositoryWrapper>, Lifestyle.Scoped);
            container.Register<IEaProvider, CacheProvider>(Lifestyle.Scoped);
            container.Register<EaProvider.EaProvider>(Lifestyle.Scoped);
            container.Register<JsonProvider>(Lifestyle.Scoped);
        }
    }
}
