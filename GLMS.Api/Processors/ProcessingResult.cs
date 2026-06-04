namespace GLMS.Processors
{
    public class ProcessingResult
    {
        public bool Success { get; set; }
        public string Message { get; init; } = string.Empty;
        public string? InvoiceNumber { get; set; }

        public static ProcessingResult Succeeded(string invoiceNumber, string message = "Request processed succesfully")
            => new() { Success = true, InvoiceNumber = invoiceNumber, Message = message };

        public static ProcessingResult Failed(string message) => new() { Success = false, Message = message };
    }
}
