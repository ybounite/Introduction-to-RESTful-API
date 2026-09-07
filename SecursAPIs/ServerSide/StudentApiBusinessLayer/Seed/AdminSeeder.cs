// using StudentDataAccessLayer;
// namespace StudentApiBusinessLayer.Seed;

// public static class AdminSeeder
// {
//   public static async Task SeedAdmin()
//   {
//     const string adminEmail = "admin@gmail.com";
//     const string passedAdmin = "admin@gmail.com";
//     // Check wether admin already exists
//       var existingAdmin = 
//         await StudentData.EmailExists(adminEmail);

//     if (existingAdmin)
//     {
//       return ;
//     }
//     // Create password hash
//     string passwordHash = 
//       BCrypt.Net.BCrypt.HashPassword(passedAdmin);

//     // Create admin student
//     var admin = new StudentDTO(
//       0,
//       "Administrator",
//       30,
//       null,
//       adminEmail
//     );

//     // Save admin with Admin role
//     await StudentData.RegisterStudent(
//       admin,
//       passwordHash,
//       "Admin"
//     );
//   }
// }