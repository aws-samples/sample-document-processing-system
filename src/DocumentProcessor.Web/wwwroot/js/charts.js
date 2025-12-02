// Chart.js initialization and management
window.charts = {};

window.initializeChart = (canvasId, type, labels, datasetLabel, data, backgroundColor) => {
    console.log('Initializing chart:', canvasId, 'type:', type, 'labels:', labels, 'data:', data);
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error('Canvas element not found:', canvasId);
        return;
    }
    console.log('Canvas element found successfully:', canvasId);

    // Destroy existing chart if it exists
    if (window.charts[canvasId]) {
        window.charts[canvasId].destroy();
    }

    const config = {
        type: type,
        data: {
            labels: labels,
            datasets: [{
                label: datasetLabel,
                data: data,
                backgroundColor: backgroundColor,
                borderColor: type === 'line' ? backgroundColor : undefined,
                borderWidth: type === 'line' ? 2 : 1,
                fill: type === 'line' ? false : undefined,
                tension: type === 'line' ? 0.1 : undefined
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: type === 'doughnut',
                    position: 'bottom'
                },
                title: {
                    display: false
                }
            },
            scales: type === 'line' ? {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            } : undefined
        }
    };

    try {
        window.charts[canvasId] = new Chart(ctx, config);
        console.log('Chart created successfully:', canvasId);
    } catch (error) {
        console.error('Error creating chart:', canvasId, error);
    }
};

window.updateChart = (canvasId, labels, data) => {
    if (window.charts[canvasId]) {
        window.charts[canvasId].data.labels = labels;
        window.charts[canvasId].data.datasets[0].data = data;
        window.charts[canvasId].update();
    }
};

window.destroyChart = (canvasId) => {
    if (window.charts[canvasId]) {
        window.charts[canvasId].destroy();
        delete window.charts[canvasId];
    }
};