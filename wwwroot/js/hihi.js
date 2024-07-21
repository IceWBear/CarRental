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
    const tableBody = document.getElementById('icebear');
    tableBody.innerHTML = '';
    cars.forEach(car => {
        const row = `<div class="col-lg-4 col-md-6 mb-4">
                            <div class="item-1">
                                <a href="#">
                                    <div class="fixed-frame">
                                        <img src="${car.image}" alt="Image" class="img-fluid">
                                    </div>
                                </a>
                                <div class="item-1-contents">
                                    <div class="text-center">
                                        <h3><a href="#"> ${car.manufacturerName} ${car.model}</a></h3>
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
                                        <a style="background-color:#008000ab;border-color:#008000ab" href="/Car/ViewDetail?car_id=${car.carId}" class="btn btn-primary"><i class="fas fa-eye"></i> View Detail </a>
                                        ${car.status ?
                `<a href="/Car/ViewDetail?car_id=${car.carId}&tab=pills-rental" class="btn btn-primary"><i class="fas fa-key"></i> Rent Now</a>` :
                `<a href="#" class="btn btn-primary disabled" tabindex="-1" aria-disabled="true">
                                        <i class="fas fa-key"></i> Rent Now
                                        </a>`
            }
                                    </div>
                                </div>
                            </div>
                        </div>
                        <style>
                            .item-1 .item-1-contents {
                              padding: 20px;
                              background: #fff; }
                              .item-1 .item-1-contents h3 {
                                font-size: 18px; }
                              .item-1 .item-1-contents .rent-price > span {
                                font-size: 1.7rem; }
                              .item-1 .item-1-contents ul {
                                list-style: none;
                                padding: 0;
                                margin: 0; }
                                .item-1 .item-1-contents ul li {
                                  margin-bottom: 5px;
                                  padding-bottom: 5px;
                                  border-bottom: 1px solid #efefef;
                                  display: block;
                                  position: relative; }
                                  .item-1 .item-1-contents ul li .spec {
                                    position: absolute;
                                    right: 0; }
                                  .item-1 .item-1-contents ul li:last-child {
                                    border-bottom: none; }
                              .item-1 .item-1-contents .action {
                                margin-top: 20px; }
                        </style>`;
        tableBody.innerHTML += row;
    });
}