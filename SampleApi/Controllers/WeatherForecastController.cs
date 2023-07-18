using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelValidation;
using SampleApi.Models;

namespace SampleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private IModelValidatorService _modelValidationService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IModelValidatorService modelValidatorService
    )
    {
        this._logger = logger;
        this._modelValidationService = modelValidatorService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost]
    public IActionResult Post(SampleCreateRequest request)
    {
        return Ok(request);
    }

    [HttpPost]
    [Route("/{id}")]
    public IActionResult PostMore([FromBody]SampleCreateRequest request, [FromQuery]SampleReadRequest id)
    {
        return Ok(request);
    }

    [HttpPut]
    public IActionResult Put(SampleUpdateRequest request)
    {
        _modelValidationService.ValidateAndThrow(request);
        return Ok(request);
    }
}
