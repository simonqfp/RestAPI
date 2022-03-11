docker stop mongo
docker volume ls
docker volume rm mongodbdata

docker ps


# create a docker volume for our MongoDb image
docker volume create --name=mongodata

# mount and start start our MongoDb image
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Password123! mongo
#docker run -d --rm mongo -p 27017:27017 -v mongodbdata:/data/db mongo

# open firewall for MongoDB port
netsh advfirewall firewall add rule name="Open mongod port 27017" dir=in action=allow protocol=TCP localport=27017

# open firewall for MongoDb exe
netsh advfirewall firewall add rule name="Allowing mongod" dir=in action=allow program=" C:\Program Files\MongoDB\Server\3.4\bin\mongod.exe"

# user secrets
dotnet user-secrets init

dotnet user-secrets set MongoDbSettings:Password Password123!