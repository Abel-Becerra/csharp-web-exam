# Security Documentation

## 🔐 Overview

This section contains all security-related documentation for the CSharp Web Exam API, including JWT authentication, authorization, and security best practices.

## 📚 Documents

### [JWT Implementation Status](JWT_IMPLEMENTATION_STATUS.md)
Complete status of JWT authentication implementation including:
- ✅ All components implemented
- ✅ Configuration details
- ✅ Test users and credentials
- ✅ Security features

## 🔒 Security Features

### JWT Authentication
- **Algorithm**: HMAC-SHA256
- **Token Duration**: 60 minutes (Development: 120 minutes)
- **Issuer**: CSharpWebExamAPI
- **Audience**: CSharpWebExamClient

### Password Security
- **Hashing**: SHA256
- **Storage**: Only hashed passwords stored
- **Validation**: Server-side validation in AuthService

### Authorization
- **Protected Endpoints**: All endpoints except `/api/auth/*`
- **Bearer Token**: Required in Authorization header
- **Automatic Validation**: On every request

## 👥 Test Users

| Username | Password | Role | Email |
|----------|----------|------|-------|
| admin | `SampleEx4mF0rT3st!ñ` | Admin | admin@example.com |
| user1 | `SampleEx4mF0rT3st!ñ` | User | user1@example.com |
| user2 | `SampleEx4mF0rT3st!ñ` | User | user2@example.com |

## 🔑 JWT Token Structure

### Claims Included
- `sub` - User ID
- `unique_name` - Username
- `email` - User email
- `role` - User role (Admin/User)
- `jti` - Unique token identifier
- `exp` - Expiration timestamp
- `iss` - Issuer
- `aud` - Audience

### Token Validation
The API validates:
- ✅ Valid signature
- ✅ Not expired
- ✅ Correct issuer
- ✅ Correct audience
- ✅ Required claims present

## 🛡️ Security Best Practices

### Implemented
- ✅ Password hashing (SHA256)
- ✅ JWT token-based authentication
- ✅ HTTPS enforcement
- ✅ CORS configuration
- ✅ Input validation
- ✅ SQL injection prevention (Dapper parameterized queries)
- ✅ Exception handling middleware
- ✅ Logging of security events

### Configuration

#### appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "CSharpWebExam-SuperSecretKey-MinLength32Characters!",
    "Issuer": "CSharpWebExamAPI",
    "Audience": "CSharpWebExamClient",
    "ExpirationMinutes": "60"
  }
}
```

#### Environment-Specific
- **Development**: 120 minutes expiration
- **QA**: 60 minutes expiration
- **Production**: 60 minutes expiration

## 🚨 Security Considerations

### Production Deployment
Before deploying to production:

1. **Change JWT Secret Key**
   - Generate a strong, random secret key (32+ characters)
   - Store in environment variables or Azure Key Vault
   - Never commit to source control

2. **Update Password Hashing**
   - Consider using BCrypt or Argon2 instead of SHA256
   - Add salt to password hashing
   - Implement password complexity requirements

3. **Enable HTTPS Only**
   - Enforce HTTPS in production
   - Disable HTTP endpoints
   - Use HSTS headers

4. **Configure CORS Properly**
   - Replace `*` with specific allowed origins
   - Restrict to known client domains

5. **Implement Rate Limiting**
   - Prevent brute force attacks on login
   - Limit API calls per user/IP

6. **Add Refresh Tokens**
   - Implement refresh token mechanism
   - Short-lived access tokens (15 min)
   - Long-lived refresh tokens (7 days)

7. **Audit Logging**
   - Log all authentication attempts
   - Log authorization failures
   - Monitor suspicious activity

## 🔍 Security Testing

### Manual Testing
1. Test authentication flow
2. Verify token expiration
3. Test with invalid tokens
4. Test with expired tokens
5. Verify authorization on protected endpoints

### Automated Testing
- Unit tests for AuthService
- Integration tests for JWT flow
- Security scanning tools
- Penetration testing

## 📖 Related Documentation

- **[JWT Usage Guide](../User/JWT_USAGE_GUIDE.md)** - How to use JWT authentication
- **[Troubleshooting](../User/TROUBLESHOOTING.md)** - Common security issues
- **[Implementation Complete](../Implementation/IMPLEMENTATION_COMPLETE.md)** - Implementation details

## 🆘 Common Security Issues

### 401 Unauthorized
**Cause**: Invalid or missing token
**Solution**: Obtain new token via `/api/auth/login`

### Token Expired
**Cause**: Token older than expiration time
**Solution**: Login again to get fresh token

### Invalid Credentials
**Cause**: Wrong username or password
**Solution**: Verify credentials or register new user

## 📞 Security Contacts

For security concerns or vulnerabilities:
- Review logs at `api/logs/api.log`
- Check [Troubleshooting Guide](../User/TROUBLESHOOTING.md)
- Consult [Implementation Status](JWT_IMPLEMENTATION_STATUS.md)

---

**Last Updated**: 2025-10-14
**Status**: ✅ Production Ready (with recommended improvements)
