using DailyStudent.Api.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace DailyStudent.Tests
{
    public class TestData
    {
        public static Institution[] InstitutionsData()
           => new[]
           {
                new Institution
                {
                    Id = 1,
                    Name = "Instituto Tecnológico de Santo Domingo",
                    Acronym = "INTEC",
                    LogoPath = "c://folder",
                    Website = "intec.edu.do",
                    CountryId = (short)1,
                    IsAvailable = true,
                    IsDeleted = false
                },
                new Institution
                {
                    Id = 2,
                    Name = "Instituto Tecnológico de las Américas",
                    Acronym = "ITLA",
                    LogoPath = "c://folder",
                    Website = "itla.edu.do",
                    CountryId = (short)1,
                    IsAvailable = true,
                    IsDeleted = false
                }
           };

        public static User[] UsersData()
           => new[]
           {
                new User
                {
                    Id = 1,
                    Email = "prueba@hotmail.com"
                }
           };
        public static Country[] ContriesData()
           => new[]
           {
                new Country
                {
                    Id = 1,
                    Iso = "RD",
                    Name = "Republica Dominicana"
                }
           };
        public static Course[] CoursesData()
           => new[]
           {
                new Course
                {
                    Id = 1,
                    Name = "Apps moviles",
                    Color = "Blue",
                    TeacherFullName = "Juan Ramírez",
                },
                new Course
                {
                    Id = 2,
                    Name = "Programación funcional",
                    Color = "Red",
                    TeacherFullName = "Joel Peralta",
                },
                new Course
                {
                    Id = 3,
                    Name = "Ionic 5",
                    Color = "Purple",
                    TeacherFullName = "Angela María Brito",
                }
           };
        public static Assignment[] AssignmentsData()
           => new[]
           {
                new Assignment
                {
                    Id = 1,
                    Title = "Resumen de AI",
                    Descripcion = "Realizar un resumen que contenta las principales caracteristicas de IA",
                    IsIndividual = true,
                    IsDeleted = false,
                    CourseId=1
                },
                new Assignment
                {
                    Id = 2,
                    Title = "Crear un Echo Server",
                    Descripcion = "Utilizando Bash o Power Shell realizar un Echo Server",
                    IsIndividual = true,
                    IsDeleted = false,
                },
                new Assignment
                {
                    Id = 3,
                    Title = "Realizar un Broadcast usando UDP",
                    Descripcion = "Mediante el protocolo UDP enviar un mensaje de broadcast a todos los routers de la red",
                    IsIndividual = true,
                    IsDeleted = false,
                }
           };
        public static Note[] NotesData()
           => new[]
           {
                new Note
                {
                    Id = 1,
                    Title = "Resumen de AI",
                    IsDeleted = false,
                },
                new Note
                {
                    Id = 2,
                    Title = "Crear un Echo Server",
                    IsDeleted = false,
                },
                new Note
                {
                    Id = 3,
                    Title = "Realizar un Broadcast usando UDP",
                    IsDeleted = false,
                }
           };
    }
}
