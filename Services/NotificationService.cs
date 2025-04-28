using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace backend.Services
{
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string EMAIL_TOPIC_ARN;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            EMAIL_TOPIC_ARN = _configuration["AWS:SNS:TopicArn"];
            
            _logger.LogInformation($"Initialized NotificationService with TopicArn: {EMAIL_TOPIC_ARN}");
        }
        
        private AmazonSimpleNotificationServiceClient GetSnsClient()
        {
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var sessionToken = _configuration["AWS:SessionToken"]; 
            var region = _configuration["AWS:Region"] ?? "us-east-1";
            
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                _logger.LogInformation("Using configured AWS credentials");
                
                AWSCredentials credentials;
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
                    _logger.LogInformation("Using session credentials");
                }
                else
                {
                    credentials = new BasicAWSCredentials(accessKey, secretKey);
                    _logger.LogInformation("Using basic credentials");
                }
                
                return new AmazonSimpleNotificationServiceClient(
                    credentials, 
                    RegionEndpoint.GetBySystemName(region)
                );
            }
            else
            {
                _logger.LogInformation("Using environment AWS credentials");
                return new AmazonSimpleNotificationServiceClient(RegionEndpoint.GetBySystemName(region));
            }
        }
        
        public async Task<string> SubscribeEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be empty");
            }
            
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format");
            }
            
            _logger.LogInformation($"Attempting to subscribe email: {email}");
            _logger.LogInformation($"Using topic ARN: {EMAIL_TOPIC_ARN}");
            
            try
            {
                using var snsClient = GetSnsClient();
                
                var subscribeRequest = new SubscribeRequest
                {
                    TopicArn = EMAIL_TOPIC_ARN,
                    Protocol = "email",
                    Endpoint = email,
                    ReturnSubscriptionArn = true
                };
                
                // Add filter policy if needed
                // subscribeRequest.Attributes = new Dictionary<string, string>
                // {
                //     { "FilterPolicy", "{\"notificationType\": [\"trade\", \"alert\", \"test\"]}" }
                // };
                
                var response = await snsClient.SubscribeAsync(subscribeRequest);
                _logger.LogInformation($"Successfully subscribed email. Subscription ARN: {response.SubscriptionArn}");
                return response.SubscriptionArn ?? "pending confirmation";
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                _logger.LogError($"AWS SNS Error: {ex.Message}");
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Status Code: {ex.StatusCode}");
                _logger.LogError($"Request ID: {ex.RequestId}");
                throw new Exception($"Failed to subscribe email: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"General Error: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                throw new Exception($"Failed to subscribe email: {ex.Message}");
            }
        }
        
        public async Task UnsubscribeAsync(string subscriptionArn)
        {
            if (string.IsNullOrEmpty(subscriptionArn) || subscriptionArn == "pending confirmation")
                return;
                
            try
            {
                using var snsClient = GetSnsClient();
                await snsClient.UnsubscribeAsync(subscriptionArn);
                _logger.LogInformation($"Successfully unsubscribed. Subscription ARN: {subscriptionArn}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error unsubscribing: {ex.Message}");
            }
        }
        
        public async Task PublishMessageAsync(string message, string subject, string notificationType = "general")
        {
            try
            {
                using var snsClient = GetSnsClient();
                
                var publishRequest = new PublishRequest
                {
                    TopicArn = EMAIL_TOPIC_ARN,
                    Message = message,
                    Subject = subject
                };
                
                // Add message attributes if needed
                publishRequest.MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { 
                        "notificationType", 
                        new MessageAttributeValue 
                        { 
                            DataType = "String", 
                            StringValue = notificationType 
                        } 
                    }
                };
                
                var response = await snsClient.PublishAsync(publishRequest);
                _logger.LogInformation($"Successfully published message to topic: {EMAIL_TOPIC_ARN}");
                _logger.LogInformation($"Message ID: {response.MessageId}");
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                _logger.LogError($"AWS SNS Error: {ex.Message}");
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Status Code: {ex.StatusCode}");
                throw new Exception($"Failed to send notification: {ex.Message}");
            }
        }
        
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}