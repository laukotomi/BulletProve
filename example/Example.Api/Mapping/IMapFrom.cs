using AutoMapper;

namespace Example.Api.Mapping
{
    /// <summary>
    /// The map from.
    /// </summary>
    public interface IMapFrom<T>
    {
        /// <summary>
        /// Mappings the.
        /// </summary>
        /// <param name="profile">The profile.</param>
        void Mapping(Profile profile)
        {
            profile.CreateMap(typeof(T), GetType());
        }
    }
}
