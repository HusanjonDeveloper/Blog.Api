﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Common.Models.JwtOptions;
using Blog.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services.Api;

public class JwtTokenService
{
    private readonly JwtOption _jwtOption = new JwtOption()
    {
        Issuer = "Blog.API",
        Audience = "Blog.Client",
        signinKey = "Default1_Default2_Default3",
        Minute = 10
    };
    
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new  Claim(ClaimTypes.Name, user.Username)
        };
            
        var signinKey = System.Text.Encoding.UTF32.GetBytes(_jwtOption.signinKey);
        
        var security = new JwtSecurityToken(issuer:_jwtOption.Issuer,audience:_jwtOption.Audience,signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(signinKey),"HS256"), expires: DateTime.Now.AddMinutes(_jwtOption.Minute),claims:claims);
        
        var token = new JwtSecurityTokenHandler().WriteToken(security);
        return token;
    }
}