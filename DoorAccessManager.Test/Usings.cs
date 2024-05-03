global using DoorAccessManager.Api;
global using DoorAccessManager.Api.Controllers;
global using DoorAccessManager.Api.Infrastructure.Authentication;
global using DoorAccessManager.Core.Services;
global using DoorAccessManager.Core.Services.Abstract;
global using DoorAccessManager.Data;
global using DoorAccessManager.Data.Repositories;
global using DoorAccessManager.Data.Repositories.Abstract;
global using DoorAccessManager.Items.Authentication;
global using DoorAccessManager.Items.Entities;
global using DoorAccessManager.Items.Enums;
global using DoorAccessManager.Items.Exceptions;
global using DoorAccessManager.Items.Models.Requests;
global using DoorAccessManager.Items.Models.Responses;
global using MapsterMapper;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Moq;
global using Newtonsoft.Json;
global using System.Text;
global using Xunit;
global using BC = BCrypt.Net.BCrypt;