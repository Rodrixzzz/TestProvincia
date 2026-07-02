using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProvincia.Application.DTOs;
using TestProvincia.Domain.Entities;

namespace TestProvincia.Application.Mappers
{
    public static class MapsHelper
    {
        public static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                DocumentType = user.DocumentType,
                DocumentNumber = user.DocumentNumber,
                Address = user.Address,
                Province = user.Province,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
