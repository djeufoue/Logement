namespace Logement.Models
{
    public class ApartmentPhoto
    {
        public long Id { get; set; }

        public long ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }

        /// <summary>
        /// Indicate which part of the apartment the photo is for (living room bed room or toilet, kitchen...)
        /// </summary>
        public string Part { get; set; }

        public string ImageURL { get; set; }
    }
}
