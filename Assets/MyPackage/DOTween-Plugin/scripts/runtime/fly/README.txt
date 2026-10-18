[module: fly]
- Tạm thời vậy đi.
=> Xem qua scene: Demo-FlyResource để biết cách sử dụng.

[Update]
- Thêm gap interval để điều chỉnh khoảng cách spawn giữa 2 gold.
- Thêm component FlyTarget để define: {icon, text}
=> component FlyTarget sẽ được add trực tiếp vào CurrencyBar để dễ dàng quản lý.
- Value Input sẽ không phải là: Coin Count hay Value Per Coin mà thay bằng: 
    + current value: số vàng hiện tại.
    + after value: số vàng sau khi nhận thưởng.
- Từ đó sẽ tính ra: value added.
- Trong config bổ sung chỉ số: MAX_COIN fly.


=> Từ: value added và MAX_COIN  => tính ra được Coin Count và Value Per Coin.

VD:
- MAX_COIN = 10.
- Giả sử: value added < 10 => ví dụ bằng 8 => thì chỉ bay ra 8 coins, mỗi coin có value = 1.
- Giả sử: value added > 10 => ví dụ 28 => Thì ta sẽ bay ra 10 coin, trong đó, 2 coin có value = 2, 8 coin có value = 3.


