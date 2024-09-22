using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Linq.Expressions ;
using System.Reflection ;

namespace Helpers.Utils ;

public static class UtProperties
{
  public static string? GetPropertyName( Expression<Func<object>> propertyExpression )
  {
    //var nameProperty = GetPropertyName( () => topCableModel.Bridge_Category) ;
    return ( propertyExpression.Body as MemberExpression )?.Member.Name ??
           ( ( propertyExpression.Body as UnaryExpression )?.Operand as MemberExpression )?.Member.Name ;
  }

  public static bool? IsNullAble( this object? obj )
  {
    if ( obj == null ) return null ;
    return Nullable.GetUnderlyingType( obj.GetType() ) != null ;
  }

  public static bool? IsChildNullAble( this object? obj, string nameProperty )
  {
    if ( obj == null ) return null ;
    var listNameProperty = nameProperty.Split( '.' ) ;
    if ( listNameProperty.Length == 1 ) {
      foreach ( var propertyInfo in obj.GetType().GetProperties() )
        if ( propertyInfo.Name == nameProperty ) {
          if ( Nullable.GetUnderlyingType( propertyInfo.PropertyType ) != null ) return true ;
          if ( propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) return true ;
          return false ;
        }

      return null ;
    }

    var valueProp = obj.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    for ( var i = 1 ; i < listNameProperty.Length ; i++ ) {
      if ( i == listNameProperty.Length - 1 )
        if ( valueProp != null ) {
          foreach ( var propertyInfo in valueProp.GetType().GetProperties() )
            if ( propertyInfo.Name == listNameProperty[ listNameProperty.Length - 1 ] ) {
              if ( Nullable.GetUnderlyingType( propertyInfo.PropertyType ) != null ) return true ;
              if ( propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) return true ;
            }

          return false ;
        }

      valueProp = valueProp?.GetType().GetProperty( listNameProperty[ i ] ) ;
    }

    return null ;
  }

  public static object? GetValuePropertyByNames( this object? obj, string? nameProperty )
  {
    if ( nameProperty == null || obj == null ) return null ;
    object? valueResult = null ;
    var listNameProperty = nameProperty.Split( '.' ) ;
    if ( listNameProperty.Length == 1 ) return obj.GetType().GetProperty( nameProperty )?.GetValue( obj ) ;
    var valueProp = obj.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    for ( var i = 1 ; i < listNameProperty.Length ; i++ ) {
      valueProp = valueProp?.GetType().GetProperty( listNameProperty[ i ] )?.GetValue( valueProp ) ;
      if ( i == listNameProperty.Length - 1 ) valueResult = valueProp ;
    }
    return valueResult ;
  }

  public static void SetValuePropertyByNames( this object? obj, string? nameProperty, object? valueSet )
  {
    if ( nameProperty == null || obj == null ) return ;
    var listNameProperty = nameProperty.Split( '.' ) ;
    var valueProp = obj.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    if ( listNameProperty.Length == 1 ) {
      obj.SetValuePropertyByName( listNameProperty[ 0 ], valueSet ) ;
      return ;
    }

    for ( var i = 1 ; i < listNameProperty.Length ; i++ ) {
      if ( i == listNameProperty.Length - 1 ) {
        valueProp.SetValuePropertyByName( listNameProperty[ listNameProperty.Length - 1 ], valueSet ) ;
        return ;
      }

      valueProp = valueProp?.GetType().GetProperty( listNameProperty[ i ] )?.GetValue( valueProp ) ;
    }
  }

  private static void SetValuePropertyByName( this object? obj, string? nameProperty, object? value )
  {
    try {
      if ( obj == null || nameProperty == null ) return ;
      PropertyInfo? property = null ;
      foreach ( var propertyInfo in obj.GetType().GetProperties() )
        if ( propertyInfo.Name == nameProperty ) {
          property = propertyInfo ;
          break ;
        }

      if ( property == null ) return ;
      if ( value == null && obj.IsChildNullAble( nameProperty ) == true ) {
        property.SetValue( obj, null ) ;
        return ;
      }

      var safeValue = obj.IsChildNullAble( nameProperty ) == true
        ? Convert.ChangeType( value, Nullable.GetUnderlyingType( property.PropertyType )! )
        : value ;
      property.SetValue( obj, safeValue ) ;
    }
    catch ( System.Exception e ) {
      LoggerCit.Instance.LogError( e ) ;
    }
  }

  public static IEnumerable<PropertyInfo> GetPropertyInfosHasTypeAttribute<TAttribute>( this object obj )
  {
    var props = obj.GetType().GetProperties().Where(
      prop => Attribute.IsDefined( prop, typeof( TAttribute ) ) ) ;
    return props ;
  }

  public static void SetValueObjectFromSameObject<T>( this T objectNeedSet, T objetValue, List<string>? listNamePropertyExtent,
    string? namePropertyContainExtent )
  {
    var allProperties = typeof( T ).GetProperties() ;
    foreach ( var propertyInfo in allProperties.ToList() ) {
      if ( listNamePropertyExtent != null && listNamePropertyExtent.Contains( propertyInfo.Name ) ) continue ;
      if ( ! string.IsNullOrEmpty( namePropertyContainExtent ) && ! propertyInfo.Name.Contains( namePropertyContainExtent ) ) continue ;
      propertyInfo.SetValue( objectNeedSet, propertyInfo.GetValue( objetValue ) ) ;
    }
  }

  public static void SetValuePropertyBaseByName<T>( this T obj, string nameProperty, object value )
  {
    PropertyInfo? property = null ;
    foreach ( var propertyInfo in typeof( T ).GetProperties() )
      if ( propertyInfo.Name == nameProperty ) {
        property = propertyInfo ;
        break ;
      }

    if ( property == null ) return ;
    var propertyType = property.PropertyType ;
    if ( property.PropertyType == typeof( string ) ) {
      var valueSet = GetValueFromObject<string>( value ) ;
      property.SetValue( obj, valueSet ) ;
      return ;
    }

    if ( property.PropertyType == typeof( double ) || property.PropertyType == typeof( double? ) ) {
      var isNullAble = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      var valueSet = isNullAble ? GetValueFromObject<double?>( value ) : GetValueFromObject<double>( value ) ;
      property.SetValue( obj, valueSet ) ;
      return ;
    }

    if ( property.PropertyType == typeof( int ) || property.PropertyType == typeof( int? ) ) {
      var isNullAble = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      var valueSet = isNullAble ? GetValueFromObject<int?>( value ) : GetValueFromObject<int>( value ) ;
      property.SetValue( obj, valueSet ) ;
      return ;
    }

    if ( property.PropertyType == typeof( bool ) || property.PropertyType == typeof( bool? ) ) {
      var isNullAble = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      var valueSet = isNullAble ? GetValueFromObject<bool?>( value ) : GetValueFromObject<bool>( value ) ;
      property.SetValue( obj, valueSet ) ;
      return ;
    }

    if ( property.PropertyType == typeof( DateTime ) || property.PropertyType == typeof( DateTime? ) ) {
      var isNullAble = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      var valueSet = isNullAble ? GetValueFromObject<DateTime?>( value ) : GetValueFromObject<DateTime>( value ) ;
      property.SetValue( obj, valueSet ) ;
      return ;
    }

    property.SetValue( obj, value ) ;
  }

  private static T? GetValueFromObject<T>( object value )
  {
    if ( typeof( T ) == typeof( string ) ) return (T) Convert.ChangeType( value.ToString(), typeof( T ) ) ;
    if ( typeof( T ) == typeof( double ) || typeof( T ) == typeof( double? ) ) {
      var iNullAble = typeof( T ).IsGenericType && typeof( T ).GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      if ( ! iNullAble ) {
        double.TryParse( value.ToString(), out var valueDouble ) ;
        return (T) Convert.ChangeType( valueDouble, typeof( T ) ) ;
      }

      if ( double.TryParse( value.ToString(), out var valueDoubleNullAble ) )
        return (T) Convert.ChangeType( valueDoubleNullAble, Nullable.GetUnderlyingType( typeof( T ) )! ) ;
      return default ;
    }

    if ( typeof( T ) == typeof( int ) || typeof( T ) == typeof( int? ) ) {
      var iNullAble = typeof( T ).IsGenericType && typeof( T ).GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      if ( ! iNullAble ) {
        int.TryParse( value.ToString(), out var valueInt ) ;
        return (T) Convert.ChangeType( valueInt, typeof( T ) ) ;
      }

      if ( int.TryParse( value.ToString(), out var valueIntNullAble ) )
        return (T) Convert.ChangeType( valueIntNullAble, Nullable.GetUnderlyingType( typeof( T ) )! ) ;
      return default ;
    }

    if ( typeof( T ) == typeof( bool ) || typeof( T ) == typeof( bool? ) ) {
      var iNullAble = typeof( T ).IsGenericType && typeof( T ).GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      if ( ! iNullAble ) {
        if ( bool.TryParse( value.ToString(), out var valueInt ) ) return (T) Convert.ChangeType( valueInt, typeof( T ) ) ;
        var valueBool = value.ToString() == "真" ;
        return (T) Convert.ChangeType( valueBool, typeof( T ) ) ;
      }

      if ( bool.TryParse( value.ToString(), out var valueIntNullAble ) )
        return (T) Convert.ChangeType( valueIntNullAble, Nullable.GetUnderlyingType( typeof( T ) )! ) ;
      if ( value.ToString() == "真" ) return (T) Convert.ChangeType( true, Nullable.GetUnderlyingType( typeof( T ) )! ) ;
      if ( value.ToString() == "偽" ) return (T) Convert.ChangeType( false, Nullable.GetUnderlyingType( typeof( T ) )! ) ;
      return default ;
    }

    if ( typeof( T ) == typeof( DateTime ) || typeof( T ) == typeof( DateTime? ) ) {
      var iNullAble = typeof( T ).IsGenericType && typeof( T ).GetGenericTypeDefinition() == typeof( Nullable<> ) ;
      if ( ! iNullAble ) {
        DateTime.TryParse( value.ToString(), out var valueInt ) ;
        return (T) Convert.ChangeType( valueInt, typeof( T ) ) ;
      }

      if ( DateTime.TryParse( value.ToString(), out var valueIntNullAble ) )
        return (T) Convert.ChangeType( valueIntNullAble, Nullable.GetUnderlyingType( typeof( T ) )! ) ;
      return default ;
    }

    return default ;
  }
}