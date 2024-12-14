import os
from groq import Groq
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from dotenv import load_dotenv
import random

# Load environment variables
load_dotenv()

# Initialize Groq client
client = Groq(
    api_key=os.environ.get("GROQ_API_KEY"),
)

def generate_review(product_name, product_category):
    # Generate varied angry subjects
    subject_prompt = f"""Generate an angry email subject line about a {product_name}.
    Make it sound like a real furious customer - use variety 
    Keep it natural and angry, no formal language.
    Write a one line subject for the email
    """
    
    # Generate matching angry body
    body_prompt = f"""Write a furious customer complaint about the {product_name}.
    Be realistic - use strong language where appropriate but not excessive.
    Write within the limit of a email body
    Write like an actual angry customer typing on their phone - casual, emotional, and direct."""
    
    try:
        # Get subject
        subject_completion = client.chat.completions.create(
            messages=[{"role": "user", "content": subject_prompt}],
            model="llama-3.1-8b-instant",
        )
        subject = subject_completion.choices[0].message.content.replace('SUBJECT:', '').strip()
        
        # Get body
        body_completion = client.chat.completions.create(
            messages=[{"role": "user", "content": body_prompt}],
            model="llama-3.1-8b-instant",
        )
        body = body_completion.choices[0].message.content.strip()
        
        return f"SUBJECT: {subject}\n\n{body}"
    except Exception as e:
        return f"Error generating review: {str(e)}"

def send_email(subject, body):
    sender_email = os.environ.get("SENDER_EMAIL")
    sender_password = os.environ.get("SENDER_PASSWORD")
    recipient_email = os.environ.get("RECIPIENT_EMAIL")

    if not all([sender_email, sender_password, recipient_email]):
        raise ValueError("Missing email configuration in environment variables")

    try:
        message = MIMEMultipart()
        message["From"] = sender_email
        message["To"] = recipient_email
        message["Subject"] = subject
        message.attach(MIMEText(body, "plain"))

        with smtplib.SMTP("smtp.gmail.com", 587) as server:
            server.starttls()
            server.login(sender_email, sender_password)
            server.send_message(message)
        
        return True
    except Exception as e:
        print(f"Error sending email: {str(e)}")
        return False

def main():
    # Check for required environment variables
    if not os.environ.get("GROQ_API_KEY"):
        raise ValueError("GROQ_API_KEY not found in environment variables")

    # List of products to review
    products = [
        {"name": "Wireless Earbuds Pro", "category": "Electronics"},
        {"name": "Ergonomic Office Chair", "category": "Furniture"},
        {"name": "Smart Coffee Maker", "category": "Kitchen Appliances"},
        {"name": "Gaming Laptop", "category": "Electronics"},
        {"name": "Standing Desk", "category": "Furniture"},
        {"name": "Noise Cancelling Headphones", "category": "Electronics"},
        {"name": "Smart Watch", "category": "Electronics"},
        {"name": "Robot Vacuum", "category": "Home Appliances"},
        {"name": "Air Purifier", "category": "Home Appliances"},
        {"name": "Gaming Mouse", "category": "Electronics"},
        {"name": "Mechanical Keyboard", "category": "Electronics"},
        {"name": "Office Desk Lamp", "category": "Furniture"},
        {"name": "Bluetooth Speaker", "category": "Electronics"},
        {"name": "Electric Toothbrush", "category": "Personal Care"},
        {"name": "Smart Thermostat", "category": "Home Appliances"},
        {"name": "Security Camera", "category": "Electronics"},
        {"name": "Blender", "category": "Kitchen Appliances"},
        {"name": "Air Fryer", "category": "Kitchen Appliances"},
        {"name": "Memory Foam Mattress", "category": "Furniture"},
        {"name": "Gaming Chair", "category": "Furniture"},
        {"name": "Wireless Charger", "category": "Electronics"},
        {"name": "Smart Light Bulbs", "category": "Home Appliances"},
        {"name": "Portable Power Bank", "category": "Electronics"},
        {"name": "Mesh WiFi System", "category": "Electronics"},
        {"name": "Electric Kettle", "category": "Kitchen Appliances"},
        {"name": "Monitor Stand", "category": "Office Accessories"},
        {"name": "Webcam", "category": "Electronics"},
        {"name": "USB-C Hub", "category": "Electronics"},
        {"name": "Desk Fan", "category": "Office Accessories"},
        {"name": "Smart Doorbell", "category": "Home Security"}
    ]

    for product in products:
        print(f"Generating review for {product['name']}...")
        review = generate_review(product["name"], product["category"])
        
        # Split into subject and body
        parts = review.split('\n', 2)
        subject = parts[0].replace('SUBJECT:', '').strip()
        body = parts[2].strip()
        
        if send_email(subject, body):
            print(f"✓ Review sent successfully!")
        else:
            print(f"✗ Failed to send review")
        print("-" * 50)

if __name__ == "__main__":
    main() 