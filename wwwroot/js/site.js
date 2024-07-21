const connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalR_Hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.error(err.toString());
});
connection.on("LoadProduct", function (product) {
    fetch('/Car/GetAllProducts')
        .then(response => response.json())
        .then(data => {
            updateProductList(data);
        });
});
function updateProductList(cars) {
    const tableBody = document.querySelector('div.tm-product-table-container');
    tableBody.innerHTML = '';
    cars.forEach(car => {
        const row = `<div class="row mb-4">
                        <div class="col-md-6">
                                <a href="#">
                                    <div class="fixed-frame">
                                        <img src="${car.image}" alt="Image" class="img-fluid">
                                    </div>
                                </a>
                            </div>
                            <div class="col-md-6">
                                <div class="item-1-contents">
                                    <div class="text-center">
                                        <h3><a href="#"> ${car.model}</a></h3>
                                        <div class="rating">
                                            <span class="icon-star text-warning"></span>
                                            <span class="icon-star text-warning"></span>
                                            <span class="icon-star text-warning"></span>
                                            <span class="icon-star text-warning"></span>
                                            <span class="icon-star text-warning"></span>
                                        </div>
                                        <div class="rent-price"><span>$ ${car.rentalPrice}/</span>day</div>
                                    </div>
                                    <ul class="specs">
                                        <li>
                                            <span>Fuel</span>
                                            <span class="spec">${car.fuel}</span>
                                        </li>
                                        <li>
                                            <span>Seats</span>
                                            <span class="spec">${car.seats}</span>
                                        </li>
                                        <li>
                                            <span>Transmission</span>
                                            <span class="spec">${car.transmission}</span>
                                        </li>
                                        <li>
                                            <span>Year of Manufacturer</span>
                                            <span class="spec">${car.year}</span>
                                        </li>
                                        <li>
                                            <span>Status</span>
                                            ${car.status ? '<span class="spec">Available</span>' : '<span class="spec">Not Available</span>'}
                                        </li>
                                    </ul>
                                    <div class="d-flex action">
                                        <a asp-action="DeleteCar" asp-route-id="${car.carId}" style="background-color:#008000ab;border-color:#008000ab" class="btn btn-primary">
                                            <i class="fas fa-trash"></i> Delete Car
                                        </a>
                                        <a asp-controller="Adminstrator" asp-action="EditCar" asp-route-id="${car.carId}" class="btn btn-primary">
                                            <i class="fas fa-pencil-alt"></i> Edit Car
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>`;
        tableBody.innerHTML += row;
    });
}