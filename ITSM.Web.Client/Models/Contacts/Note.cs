using System.ComponentModel.DataAnnotations;

namespace ITSM.Web.Client.Models;

public class Note {
    [Required]
    public string? Text { get; set; }
    public DateTime? Date { get; set; }
    public string? Manager { get; set; }
}
