namespace Logement.Models
{
    public class ApartmentPhoto
    {
        public long Id { get; set; }

        /// <summary>
        /// Removed because i don't know why an ApartmentPhoto will need a File(The contrat)
        /// </summary>
/*      public long FileId { get; set; }
        public virtual FileModel File { get; set; }*/

        public long ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }

        /// <summary>
        /// Indicate which part of the apartment the photo is for (living room bed room or toilet, kitchen...)
        /// </summary>
        public string Part { get; set; }
    }
}
