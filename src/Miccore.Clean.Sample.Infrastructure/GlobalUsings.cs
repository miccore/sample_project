// ===========================================
// Global using directives for Miccore.Clean.Sample.Infrastructure
// ===========================================

// System namespaces
global using System.Linq.Expressions;

// Microsoft namespaces
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// External packages
global using Miccore.Pagination;

// Project namespaces - Core
global using Miccore.Clean.Sample.Core.Configurations;
global using Miccore.Clean.Sample.Core.Entities;
global using Miccore.Clean.Sample.Core.Entities.Base;
global using Miccore.Clean.Sample.Core.Enums;
global using Miccore.Clean.Sample.Core.Exceptions;
global using Miccore.Clean.Sample.Core.Extensions;
global using Miccore.Clean.Sample.Core.Helpers;
global using Miccore.Clean.Sample.Core.Interfaces;
global using Miccore.Clean.Sample.Core.Repositories;
global using Miccore.Clean.Sample.Core.Repositories.Base;

// Project namespaces - Infrastructure
global using Miccore.Clean.Sample.Infrastructure.Persistance;
global using Miccore.Clean.Sample.Infrastructure.Repositories;
global using Miccore.Clean.Sample.Infrastructure.Repositories.Base;
