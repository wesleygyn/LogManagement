﻿using AutoMapper;
using LogManagement.Models;
using LogManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManagement.Mappings
{
    public class EntitiesToDTOMappigProfile : Profile
    {
        public EntitiesToDTOMappigProfile()
        {
            CreateMap<Usuario, UserManagerViewModel>();

        }
    }
}
