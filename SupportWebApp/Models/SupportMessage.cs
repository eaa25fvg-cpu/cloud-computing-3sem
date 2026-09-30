using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SupportWebApp.Models;

public enum SupportCategory
{
    TechnicalQuestionBeforePurchase,
    SpareParts,
    ProductSuggestion,
    FindDealer,
    RequestCatalog,
    Other
}

public class SupportMessage
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "Navn skal udfyldes.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail skal udfyldes.")]
    [EmailAddress(ErrorMessage = "Indtast en gyldig e-mailadresse.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefonnummer skal udfyldes.")]
    [Phone(ErrorMessage = "Indtast et gyldigt telefonnummer.")]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivelse skal udfyldes.")]
    [StringLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vælg en kategori.")]
    public SupportCategory? Category { get; set; }

    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
}
