# Instructions to Run the Application
To run the application follows these steps:

First clone the repo
```bash
   git clone https://github.com/AndreasBerthelsen/Mini_project_Secure_Software_Develoment
```
Then navigate to the folder and run this command:
```bash
   dotnet run
```

# Screenshots of the product.
![passwordManager1](https://github.com/user-attachments/assets/2720b302-3d7a-4e68-bb27-fcfeedd9fa08)
![passwordManager2](https://github.com/user-attachments/assets/11474c10-909d-47c6-b0a0-a6fef4ab4008)

# Discussion about security of your product.
### What do you protect against (who are the threat actors)
The actors I protect against are:
People who are trying to gain physical or remote access unauthorized access to different app/services, by stealing credentials.

### What is your security model (encryption, key handling etc.)
The master password is never stored. When a new password is created, I use the static class EncryptionService, and the InitializeAsync method, to derive a key using Argon2id. 

When new passwords are created, they get stored in a database as an encrypted string. The encryption key, is derived from the master key. 

When a user wants to valide the master password, I get a list of the passwords from the database, then take an item from that list, and tries to decrypt it, by using the entered master password, to derive the key, with the master password, to ensure that the master password matches, the master password created to encrypt the passwords. 
The encryption key is never stored, but rather created when needed.

### Any pitfalls or limitations in your solution.
#### Zero Knowledge Encryption:
The solution uses a zero-knowledge encryption model, meaning the system never stores or has access to the master password or the derived encryption keys. While this enhances security and privacy, it also introduces a limitation: if the user forgets their master password, there is no way to recover it or the encrypted data. This can lead to permanent data loss.

#### Single Point of Failure:
The master password serves as a single point of failure. If it's compromised, an attacker could potentially derive the encryption key and decrypt all stored data. The entire security of the system depends on the strength and protection of the master password, so it is crucial to ensure that it is strong and securely managed by the user.