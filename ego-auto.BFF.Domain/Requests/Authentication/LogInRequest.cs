namespace ego_auto.BFF.Domain.Requests.Authentication;

public record LogInRequest(string? Email = null, string? Password = null);
