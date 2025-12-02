// File download functions for DocumentViewer
window.fileOperations = {
    // Download file from base64 content
    downloadFromBase64: function (fileName, base64Content, contentType) {
        try {
            // Convert base64 to blob
            const byteCharacters = atob(base64Content);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: contentType || 'application/octet-stream' });
            
            // Create download link
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            
            // Cleanup
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
            
            return true;
        } catch (error) {
            console.error('Error downloading file:', error);
            return false;
        }
    },
    
    // Download file from byte array
    downloadFromBytes: function (fileName, byteArray, contentType) {
        try {
            const blob = new Blob([byteArray], { type: contentType || 'application/octet-stream' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
            return true;
        } catch (error) {
            console.error('Error downloading file:', error);
            return false;
        }
    },
    
    // Export JSON data
    exportJson: function (fileName, jsonData) {
        try {
            const blob = new Blob([jsonData], { type: 'application/json' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
            return true;
        } catch (error) {
            console.error('Error exporting JSON:', error);
            return false;
        }
    }
};