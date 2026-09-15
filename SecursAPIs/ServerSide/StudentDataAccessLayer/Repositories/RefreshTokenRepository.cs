using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentDataAccessLayer.Interfaces;
using StudentDataAccessLayer.Models;

namespace StudentDataAccessLayer.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
	private readonly string _connectionString = string.Empty;
  public RefreshTokenRepository(IConfiguration configuration)
  {
    _connectionString = 
      configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
          "DefaultConnection was not found."
        );
  }
	public async Task CreateAsync(RefreshToken refreshToken)
	{
		await using var connection = new SqlConnection(_connectionString);
		await using var command = new SqlCommand(
			@"INSERT INTO RefreshToken(
				UserId,
				TokenHash,
				RefreshTokenExpiresAt,
				RefreshTokenRevokedAt
				)
				VALUES(
				@UserId,
				@TokenHash,
				@RefreshTokenExpiresAt,
				@RefreshTokenRevokedAt
				)",
				connection
		);
		
		command.Parameters.AddWithValue("@UserId", refreshToken.UserId);
		command.Parameters.AddWithValue("@TokenHash", refreshToken.TokenHash);
		command.Parameters.AddWithValue(
				"@RefreshTokenExpiresAt",
				refreshToken.RefreshTokenExpiresAt);
		command.Parameters.AddWithValue(
				"@RefreshTokenRevokedAt",
				(object?)refreshToken.RefreshTokenRevokedAt ?? DBNull.Value);
			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();
	}

	public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
	{
		await using var connection = new SqlConnection(_connectionString);
		await using var command = new SqlCommand(
			@"SELECT Id, UserId, TokenHash, RefreshTokenExpiresAt, RefreshTokenRevokedAt
      FROM RefreshToken WHERE TokenHash = @tokenHash",
			connection
		);

	
		command.Parameters.AddWithValue(
				"@TokenHash", tokenHash);
		await connection.OpenAsync();
		using var reader = await command.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			// object mapping
			return new RefreshToken
			{
				Id = Convert.ToInt32(reader["Id"]),
				UserId = Convert.ToInt32(reader["UserId"]),
				TokenHash = reader["TokenHash"]?.ToString() ?? "",
				RefreshTokenExpiresAt = Convert.ToDateTime(reader["RefreshTokenExpiresAt"]),
				RefreshTokenRevokedAt = 
					reader["RefreshTokenRevokedAt"] == DBNull.Value 
						? null : Convert.ToDateTime(reader["RefreshTokenRevokedAt"])
			};
		}
		return null;
	}

	//public Task RevokeAsync(int refreshTokenId, DateTime revokedAt)
	//{
		
	//}
}