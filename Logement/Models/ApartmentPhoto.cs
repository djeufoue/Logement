namespace Logement.Models
{
    public class ApartmentPhoto
    {
        public int Id { get; set; }

        public int FileId { get; set; }
        public virtual FileModel File { get; set; }

        public int ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }

        /// <summary>
        /// Indicate which part of the apartment the photo is for (living room bed room or toilet, kitchen...)
        /// </summary>
        public string Part { get; set; }
    }
}
