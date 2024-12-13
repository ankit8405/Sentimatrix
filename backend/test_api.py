import requests
import json

BASE_URL = "http://localhost:5000/api"

def test_email(subject, body, sender, receiver, print_response=True):
    print(f"\nSending email: {subject}")
    
    data = {
        "subject": subject,
        "body": body,
        "senderEmail": sender,
        "receiverEmail": receiver
    }
    
    try:
        response = requests.post(f"{BASE_URL}/EmailProcess", data=data)
        print(f"Response status: {response.status_code}")
        
        if response.status_code != 200:
            print(f"Error: {response.text}")
        elif print_response:
            print(json.dumps(response.json(), indent=2))
            
        return response
        
    except Exception as e:
        print(f"Error: {str(e)}")
        return None

def get_emails(endpoint):
    try:
        response = requests.get(f"{BASE_URL}/Email/{endpoint}")
        return response.json()
    except Exception as e:
        print(f"Error: {str(e)}")
        return None

# Test positive email
test_email(
    "Great News About Project",
    "I'm really excited to share that we've made excellent progress on the project. The team has done an amazing job!",
    "alice@example.com",
    "bob@example.com"
)

# Test negative email
test_email(
    "Urgent: Server Issues",
    "The production server is down and customers are complaining. This is a critical issue that needs immediate attention!",
    "bob@example.com",
    "alice@example.com"
)

# Test neutral email
test_email(
    "Meeting Follow-up",
    "Here are the action items from our meeting. Please review and let me know if you have any questions.",
    "charlie@example.com",
    "team@example.com"
)

# Get all emails
print("\nAll Emails:")
all_emails = get_emails("")
if all_emails:
    print(json.dumps(all_emails, indent=2))

# Get positive emails
print("\nPositive Emails:")
positive_emails = get_emails("positive")
if positive_emails:
    print(json.dumps(positive_emails, indent=2))

# Get negative emails
print("\nNegative Emails:")
negative_emails = get_emails("negative")
if negative_emails:
    print(json.dumps(negative_emails, indent=2))
