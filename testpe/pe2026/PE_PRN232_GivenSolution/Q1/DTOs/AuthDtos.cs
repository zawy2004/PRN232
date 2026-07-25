namespace Q1.DTOs
{
    public record LoginRequestDto(string Email, string Password);
    public record LoginResponseDto(string Token, string Email, string Role, int UserId);
}
