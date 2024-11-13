#!/bin/bash

# Configuration
CONFIGURATION="Release"
DAEMON_EXE="Driver"
DAEMON_FOLDER="/home/cdl"
DAEMON_PATH="$DAEMON_FOLDER/$DAEMON_EXE"
SERVICE_NAME="cdl2024-driver.service"
SERVICE_PATH="/etc/systemd/system/$SERVICE_NAME"
REMOTE_USER="cdl"
REMOTE_HOST="definehost"
ARCH="linux-arm64"

# Environment variables
SERIAL_NUMBER="DEFINE_SERIALNUMBER"
SERVER_BASE_URL="http://192.168.42.4:8080"
NET_INTERFACE_NAME="eth0"
REFRESH_RATE="1000"

build_daemon() {
    echo "Building daemon executable..."
    dotnet publish -c "$CONFIGURATION" -r $ARCH --self-contained true -p:PublishSingleFile=true
}

# Function to upload the daemon executable
upload_daemon() {
    echo "Uploading daemon executable..."
    rsync -avz "./bin/$CONFIGURATION/net8.0/$ARCH/publish/$DAEMON_EXE" "$REMOTE_USER@$REMOTE_HOST:$DAEMON_PATH"
}

# Function to create the systemd service file
create_service() {
    echo "Creating systemd service file..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo tee $SERVICE_PATH > /dev/null" <<EOF
[Unit]
Description=CDL2024 Driver Service
After=network.target

[Service]
ExecStart=$DAEMON_PATH
Restart=always
User=root
Environment="SERIAL_NUMBER=$SERIAL_NUMBER"
Environment="SERVER_BASE_URL=$SERVER_BASE_URL"
Environment="NET_INTERFACE_NAME=$NET_INTERFACE_NAME"
Environment="REFRESH_RATE=$REFRESH_RATE"

[Install]
WantedBy=multi-user.target
EOF
}

# Function to reload systemd and restart the service
restart_service() {
    echo "Reloading systemd and restarting the service..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl daemon-reload"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl restart $SERVICE_NAME"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl enable $SERVICE_NAME"
}

# Function to verify the service status
verify_service() {
    echo "Verifying service status..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl status $SERVICE_NAME"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo journalctl -fu $SERVICE_NAME"
}

# Main script
build_daemon
upload_daemon
create_service
restart_service
verify_service
